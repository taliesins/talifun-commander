using System;
using System.Threading;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Client.ResumableUpload;
using Google.GData.YouTube;
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

			var executedYouTubeUploaderWorkflowMessage = new ExecutedYouTubeUploaderWorkflowMessage()
			{
				CorrelationId = executeYouTubeUploaderWorkflowMessage.CorrelationId,
				Cancelled = e.Cancelled,
				Error = e.Error
			};

			var bus = BusDriver.Instance.GetBus(YouTubeUploaderService.BusName);
			bus.Publish(executedYouTubeUploaderWorkflowMessage);
		}
	}
}
