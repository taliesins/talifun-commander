using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Token;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using Talifun.Commander.Command.DropBoxUploader.Command.Request;
using Talifun.Commander.Command.DropBoxUploader.Command.Response;
using Talifun.Commander.Command.DropBoxUploader.Command.Settings;
using Talifun.Commander.Command.Esb;
using MassTransit;

namespace Talifun.Commander.Command.DropBoxUploader.Command
{
	public abstract class ExecuteDropBoxUploaderWorkflowMessageHandlerBase
	{
		private DropBoxConfiguration GetDropBoxConfiguration()
		{
			var config = DropBoxConfiguration.GetStandardConfiguration();
			config.AuthorizationCallBack = "http://www.talifun.com/oauth/dropbox.htm";
			return config;
		}

		protected DropBoxToken CheckAuthenticationToken(IAuthenticationSettings authenticationSettings)
		{
			var requestToken = new DropBoxBaseTokenInformation()
			{
				ConsumerKey = authenticationSettings.DropBoxApiKey,
				ConsumerSecret = authenticationSettings.DropBoxApiSecret
			};
			var authenticationToken = new OAuthToken(authenticationSettings.DropBoxAuthenticationKey, authenticationSettings.DropBoxAuthenticationSecret);
			var accessToken = new DropBoxToken(authenticationToken, requestToken);
			return accessToken;
		}

		protected void ExecuteUpload(IExecuteDropBoxUploaderWorkflowMessage message, DropBoxToken authenticationToken, FileInfo inputFilePath)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var task = Task.Factory.StartNew(() => { });
			task.ContinueWith((t) =>
			{
				DropBoxUploaderService.Uploaders.Add(message, new CancellableTask
				{
					Task = task,
					CancellationTokenSource = cancellationTokenSource
				});

				var configuration = GetDropBoxConfiguration();
				var storage = new CloudStorage();
				storage.Open(configuration, authenticationToken);

				var folder = string.IsNullOrEmpty(message.Settings.Folder) ? storage.GetRoot() : storage.GetFolder(message.Settings.Folder);
				if (folder == null)
				{
					throw new Exception(string.Format("Folder does not exist - {0}", message.Settings.Folder));
				}
				else
				{
					var file = storage.CreateFile(folder, inputFilePath.Name);
					var uploader = file.GetDataTransferAccessor();
					using (var inputFileStream = inputFilePath.OpenRead())
					{
						uploader.Transfer(inputFileStream, nTransferDirection.nUpload, FileOperationProgressChanged, task);
					}
				}

				if (storage.IsOpened)
				{
					storage.Close();
				}
			}
			, cancellationToken)
			.ContinueWith((t) =>
			{
				var executedDropBoxUploaderWorkflowMessage = new ExecutedDropBoxUploaderWorkflowMessage()
				{
					CorrelationId = message.CorrelationId,
					Cancelled = t.IsCanceled,
					Error = t.Exception
				};

				var bus = BusDriver.Instance.GetBus(DropBoxUploaderService.BusName);
				bus.Publish(executedDropBoxUploaderWorkflowMessage);

				DropBoxUploaderService.Uploaders.Remove(message);
			});
		}

		private void FileOperationProgressChanged(object sender, FileDataTransferEventArgs e)
		{
			var task = (Task)e.CustomnContext;
			
			if (task.IsCanceled)
			{
				e.Cancel = true;
			}
		}
	}
}
