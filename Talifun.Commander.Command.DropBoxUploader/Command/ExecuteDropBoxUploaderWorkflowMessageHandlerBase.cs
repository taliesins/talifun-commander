using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.DropBoxUploader.Command.Events;
using Talifun.Commander.Command.DropBoxUploader.Command.Request;
using Talifun.Commander.Command.DropBoxUploader.Command.Response;
using Talifun.Commander.Command.DropBoxUploader.Command.Settings;

namespace Talifun.Commander.Command.DropBoxUploader.Command
{
	public abstract class ExecuteDropBoxUploaderWorkflowMessageHandlerBase
	{
		//protected void ExecuteUpload(IExecuteDropBoxUploaderWorkflowMessage message, DropBoxAuthenticator flickrAuthenticator, FileInfo inputFilePath)
		//{
		//    var cancellationTokenSource = new CancellationTokenSource();
		//    var cancellationToken = cancellationTokenSource.Token;

		//    var task = Task.Factory.StartNew(() => { });
		//    task.ContinueWith((t) =>
		//    {
		//        DropBoxUploaderService.Uploaders.Add(message, new CancellableTask
		//        {
		//            Task = task,
		//            CancellationTokenSource = cancellationTokenSource
		//        });

		//        var uploader = new Uploader(flickrAuthenticator);
		//        uploader.AsyncOperationCompleted += OnResumableUploaderAsyncOperationCompleted;
		//        uploader.AsyncOperationProgress += OnResumableUploaderAsyncOperationProgress;
		//        if (!cancellationToken.IsCancellationRequested)
		//        {
		//            cancellationToken.Register(() => uploader.CancelAsync(message));
		//            var inputFileStream = inputFilePath.OpenRead();

		//            uploader.UploadPictureAsync(inputFileStream, inputFilePath.Name,
		//                                        message.Settings.MetaData.Title,
		//                                        message.Settings.MetaData.Description,
		//                                        message.Settings.MetaData.Keywords,
		//                                        message.Settings.MetaData.IsPublic,
		//                                        message.Settings.MetaData.IsFamily,
		//                                        message.Settings.MetaData.IsFriend,
		//                                        message.Settings.MetaData.ContentType,
		//                                        message.Settings.MetaData.SafetyLevel,
		//                                        message.Settings.MetaData.HiddenFromSearch,
		//                                        message);
		//        }
		//        cancellationToken.ThrowIfCancellationRequested();
		//    }
		//        , cancellationToken)
		//    .ContinueWith((t) =>
		//    {
		//        DropBoxUploaderService.Uploaders.Remove(message);
		//    });
		//}

		//protected void OnResumableUploaderAsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
		//{
		//    var message = string.Format("Upload Progress: {0} ({1}/{2} - {3}%) {4} {5}", DateTime.Now, e.Position, e.CompleteSize,
		//                                e.ProgressPercentage, e.HttpVerb, e.Uri);

		//    var executeDropBoxUploaderWorkflowMessage = (IExecuteDropBoxUploaderWorkflowMessage)e.UserState;

		//    var flickrUploaderProgressMessage = new DropBoxUploaderProgressMessage
		//    {
		//        CorrelationId = executeDropBoxUploaderWorkflowMessage.CorrelationId,
		//        InputFilePath = executeDropBoxUploaderWorkflowMessage.InputFilePath,
		//        Output = message
		//    };

		//    var bus = BusDriver.Instance.GetBus(DropBoxUploaderService.BusName);
		//    bus.Publish(flickrUploaderProgressMessage);
		//}


		//protected void OnResumableUploaderAsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
		//{
		//    var executeDropBoxUploaderWorkflowMessage = (IExecuteDropBoxUploaderWorkflowMessage)e.UserState;

		//    if (e.Error != null)
		//    {
		//        var webException = (WebException)e.Error;
		//        var response = webException.Response;
		//        var responseBody = string.Empty;

		//        if (response != null)
		//        {
		//            if (response.ContentLength > 0)
		//            {
		//                using (var stream = response.GetResponseStream())
		//                {
		//                    using (var reader = new StreamReader(stream))
		//                    {
		//                        responseBody = reader.ReadToEnd().Trim();
		//                    }
		//                }
		//            }
		//        }
		//    }

		//    var executedDropBoxUploaderWorkflowMessage = new ExecutedDropBoxUploaderWorkflowMessage()
		//    {
		//        CorrelationId = executeDropBoxUploaderWorkflowMessage.CorrelationId,
		//        Cancelled = e.Cancelled,
		//        Error = e.Error
		//    };

		//    var bus = BusDriver.Instance.GetBus(DropBoxUploaderService.BusName);
		//    bus.Publish(executedDropBoxUploaderWorkflowMessage);
		//}

		//protected Auth CheckAuthenticationToken(IAuthenticationSettings authenticationSettings)
		//{
		//    if (string.IsNullOrEmpty(authenticationSettings.DropBoxPassword))
		//    {
		//        throw new Exception("No DropBox AuthToken!");
		//    }

		//    DropBox.CacheTimeout = new TimeSpan(1, 0, 0, 0, 0);

		//    var flickrService = new DropBox(authenticationSettings.DropBoxApiKey, authenticationSettings.DropBoxApiSecret, authenticationSettings.DropBoxPassword);
		//    var authenticationToken = flickrService.AuthCheckToken(authenticationSettings.DropBoxPassword);

		//    return authenticationToken;
		//}
	}
}
