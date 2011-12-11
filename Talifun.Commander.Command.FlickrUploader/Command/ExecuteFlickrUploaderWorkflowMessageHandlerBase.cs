using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using FlickrNet.Uploader;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Events;
using Talifun.Commander.Command.FlickrUploader.Command.Request;
using Talifun.Commander.Command.FlickrUploader.Command.Response;
using Talifun.Commander.Command.FlickrUploader.Command.Settings;

namespace Talifun.Commander.Command.FlickrUploader.Command
{
	public abstract class ExecuteFlickrUploaderWorkflowMessageHandlerBase
	{
		protected void ExecuteUpload(IExecuteFlickrUploaderWorkflowMessage message, FlickrAuthenticator flickrAuthenticator, FileInfo inputFilePath)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var task = Task.Factory.StartNew(() => { });
			task.ContinueWith((t) =>
			{
				FlickrUploaderService.Uploaders.Add(message, new CancellableTask
				{
					Task = task,
					CancellationTokenSource = cancellationTokenSource
				});

				var uploader = new Uploader(flickrAuthenticator);
				uploader.AsyncOperationCompleted += OnResumableUploaderAsyncOperationCompleted;
				uploader.AsyncOperationProgress += OnResumableUploaderAsyncOperationProgress;
				if (!cancellationToken.IsCancellationRequested)
				{
					cancellationToken.Register(() => uploader.CancelAsync(message));
					var inputFileStream = inputFilePath.OpenRead();

					uploader.UploadPictureAsync(inputFileStream, inputFilePath.Name,
					                            message.Settings.MetaData.Title,
					                            message.Settings.MetaData.Description,
					                            message.Settings.MetaData.Keywords,
					                            message.Settings.MetaData.IsPublic,
					                            message.Settings.MetaData.IsFamily,
					                            message.Settings.MetaData.IsFriend,
					                            message.Settings.MetaData.ContentType,
					                            message.Settings.MetaData.SafetyLevel,
					                            message.Settings.MetaData.HiddenFromSearch,
					                            message);
				}
				cancellationToken.ThrowIfCancellationRequested();
			}
				, cancellationToken)
			.ContinueWith((t) =>
			{
				FlickrUploaderService.Uploaders.Remove(message);
			});
		}

		protected void OnResumableUploaderAsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
		{
			var message = string.Format("Upload Progress: {0} ({1}/{2} - {3}%) {4} {5}", DateTime.Now, e.Position, e.CompleteSize,
										e.ProgressPercentage, e.HttpVerb, e.Uri);

			var executeFlickrUploaderWorkflowMessage = (IExecuteFlickrUploaderWorkflowMessage)e.UserState;

			var flickrUploaderProgressMessage = new FlickrUploaderProgressMessage
			{
				CorrelationId = executeFlickrUploaderWorkflowMessage.CorrelationId,
				InputFilePath = executeFlickrUploaderWorkflowMessage.InputFilePath,
				Output = message
			};

			var bus = BusDriver.Instance.GetBus(FlickrUploaderService.BusName);
			bus.Publish(flickrUploaderProgressMessage);
		}

		protected void OnResumableUploaderAsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
		{
			var executeFlickrUploaderWorkflowMessage = (IExecuteFlickrUploaderWorkflowMessage)e.UserState;

			if (e.Error != null)
			{
				var webException = (WebException)e.Error;
				var response = webException.Response;
				var responseBody = string.Empty;

				if (response != null)
				{
					if (response.ContentLength > 0)
					{
						using (var stream = response.GetResponseStream())
						{
							using (var reader = new StreamReader(stream))
							{
								responseBody = reader.ReadToEnd().Trim();
							}
						}
					}
				}
			}

			var executedFlickrUploaderWorkflowMessage = new ExecutedFlickrUploaderWorkflowMessage()
			{
				CorrelationId = executeFlickrUploaderWorkflowMessage.CorrelationId,
				Cancelled = e.Cancelled,
				Error = e.Error
			};

			var bus = BusDriver.Instance.GetBus(FlickrUploaderService.BusName);
			bus.Publish(executedFlickrUploaderWorkflowMessage);
		}

		protected Auth CheckAuthenticationToken(IAuthenticationSettings authenticationSettings)
		{
			if (string.IsNullOrEmpty(authenticationSettings.FlickrAuthToken))
			{
				throw new Exception("No Flickr AuthToken!");
			}

			Flickr.CacheTimeout = new TimeSpan(1, 0, 0, 0, 0);

			var flickrService = new Flickr(authenticationSettings.FlickrApiKey, authenticationSettings.FlickrApiSecret, authenticationSettings.FlickrAuthToken);
			var authenticationToken = flickrService.AuthCheckToken(authenticationSettings.FlickrAuthToken);

			return authenticationToken;
		}
	}
}
