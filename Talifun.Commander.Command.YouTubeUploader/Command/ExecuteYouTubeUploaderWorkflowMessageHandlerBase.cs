using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.YouTube;
using Google.YouTube;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.YouTubeUploader.Command.Events;
using Talifun.Commander.Command.YouTubeUploader.Command.Request;
using Talifun.Commander.Command.YouTubeUploader.Command.Response;

namespace Talifun.Commander.Command.YouTubeUploader.Command
{
	public abstract class ExecuteYouTubeUploaderWorkflowMessageHandlerBase
	{
		protected void ExecuteUpload(IExecuteYouTubeUploaderWorkflowMessage message, ClientLoginAuthenticator youTubeAuthenticator, YouTubeEntry youTubeEntry)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var task = Task.Factory.StartNew(()=>{});
			task.ContinueWith((t) =>
				{
					YouTubeUploaderService.Uploaders.Add(message, new CancellableTask
					{
						Task = task,
						CancellationTokenSource = cancellationTokenSource
					});

					var resumableUploader = new ResumableUploader(message.Settings.Upload.ChunkSize);
					resumableUploader.AsyncOperationCompleted += OnResumableUploaderAsyncOperationCompleted;
					resumableUploader.AsyncOperationProgress += OnResumableUploaderAsyncOperationProgress;
					if (!cancellationToken.IsCancellationRequested)
					{
						cancellationToken.Register(() => resumableUploader.CancelAsync(message));
						resumableUploader.InsertAsync(youTubeAuthenticator, youTubeEntry, message);
					}
					cancellationToken.ThrowIfCancellationRequested();
				}
				, cancellationToken)
			.ContinueWith((t) =>
			{
				YouTubeUploaderService.Uploaders.Remove(message);
			});
		}

		protected void OnResumableUploaderAsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
		{
			var message = string.Format("Upload Progress: {0} ({1}/{2} - {3}%) {4} {5}", DateTime.Now, e.Position, e.CompleteSize,
										e.ProgressPercentage, e.HttpVerb, e.Uri);

			var executeYouTubeUploaderWorkflowMessage = (IExecuteYouTubeUploaderWorkflowMessage)e.UserState;

			var youTubeUploaderProgressMessage = new YouTubeUploaderProgressMessage
			{
				CorrelationId = executeYouTubeUploaderWorkflowMessage.CorrelationId,
				InputFilePath = executeYouTubeUploaderWorkflowMessage.InputFilePath,
				Output = message
			};

			var bus = BusDriver.Instance.GetBus(YouTubeUploaderService.BusName);
			bus.Publish(youTubeUploaderProgressMessage);
		}

		protected void OnResumableUploaderAsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
		{
			var executeYouTubeUploaderWorkflowMessage = (IExecuteYouTubeUploaderWorkflowMessage)e.UserState;

			if (e.Error != null)
			{
				var webException = (WebException) e.Error;
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

			var videoId = string.Empty;

			if (!e.Cancelled && e.Error == null)
			{
				var youTubeRequestSettings = new YouTubeRequestSettings(executeYouTubeUploaderWorkflowMessage.Settings.Authentication.ApplicationName, executeYouTubeUploaderWorkflowMessage.Settings.Authentication.DeveloperKey);
				var youTubeRequest = new YouTubeRequest(youTubeRequestSettings);
				var video = youTubeRequest.ParseVideo(e.ResponseStream);
				videoId = video.VideoId;
			}

			var executedYouTubeUploaderWorkflowMessage = new ExecutedYouTubeUploaderWorkflowMessage()
			{
				CorrelationId = executeYouTubeUploaderWorkflowMessage.CorrelationId,
				Cancelled = e.Cancelled,
				Error = e.Error,
				VideoId = videoId
			};

			var bus = BusDriver.Instance.GetBus(YouTubeUploaderService.BusName);
			bus.Publish(executedYouTubeUploaderWorkflowMessage);
		}
	}
}
