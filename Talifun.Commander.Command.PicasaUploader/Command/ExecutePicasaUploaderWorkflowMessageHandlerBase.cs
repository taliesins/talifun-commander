using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.Photos;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command.Events;
using Talifun.Commander.Command.PicasaUploader.Command.Request;
using Talifun.Commander.Command.PicasaUploader.Command.Response;

namespace Talifun.Commander.Command.PicasaUploader.Command
{
	public abstract class ExecutePicasaUploaderWorkflowMessageHandlerBase
	{
		protected void ExecuteUpload(IExecutePicasaUploaderWorkflowMessage message, ClientLoginAuthenticator picasaAuthenticator, PicasaEntry picasaEntry)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var task = Task.Factory.StartNew(()=>{});
			task.ContinueWith((t) =>
				{
					if (!cancellationToken.IsCancellationRequested)
					{
						PicasaUploaderService.Uploaders.Add(message, new CancellableTask
						{
							Task = task,
							CancellationTokenSource = cancellationTokenSource
						});

						var resumableUploader = new ResumableUploader(message.Settings.Upload.ChunkSize);

						resumableUploader.AsyncOperationCompleted += OnResumableUploaderAsyncOperationCompleted;
						resumableUploader.AsyncOperationProgress += OnResumableUploaderAsyncOperationProgress;

						cancellationToken.Register(() => resumableUploader.CancelAsync(message));
						resumableUploader.InsertAsync(picasaAuthenticator, picasaEntry, message);
					}
					cancellationToken.ThrowIfCancellationRequested();
				}
				, cancellationToken);
		}

		protected void OnResumableUploaderAsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
		{
			var message = string.Format("Upload Progress: {0} ({1}/{2} - {3}%) {4} {5}", DateTime.Now, e.Position, e.CompleteSize,
										e.ProgressPercentage, e.HttpVerb, e.Uri);

			var executePicasaUploaderWorkflowMessage = (IExecutePicasaUploaderWorkflowMessage)e.UserState;

			var picasaUploaderProgressMessage = new PicasaUploaderProgressMessage
			{
				CorrelationId = executePicasaUploaderWorkflowMessage.CorrelationId,
				InputFilePath = executePicasaUploaderWorkflowMessage.InputFilePath,
				Output = message
			};

			var bus = BusDriver.Instance.GetBus(PicasaUploaderService.BusName);
			bus.Publish(picasaUploaderProgressMessage);
		}

		protected void OnResumableUploaderAsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
		{
			var executePicasaUploaderWorkflowMessage = (IExecutePicasaUploaderWorkflowMessage)e.UserState;

			PicasaUploaderService.Uploaders.Remove(executePicasaUploaderWorkflowMessage);

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

			var executedPicasaUploaderWorkflowMessage = new ExecutedPicasaUploaderWorkflowMessage()
			{
				CorrelationId = executePicasaUploaderWorkflowMessage.CorrelationId,
				Cancelled = e.Cancelled,
				Error = e.Error
			};

			var bus = BusDriver.Instance.GetBus(PicasaUploaderService.BusName);
			bus.Publish(executedPicasaUploaderWorkflowMessage);
		}
	}
}
