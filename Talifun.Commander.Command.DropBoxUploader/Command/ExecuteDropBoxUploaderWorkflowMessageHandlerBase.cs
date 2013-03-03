using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using Talifun.Commander.Command.DropBoxUploader.Command.Request;
using Talifun.Commander.Command.DropBoxUploader.Command.Response;
using Talifun.Commander.Command.DropBoxUploader.Command.Settings;
using Talifun.Commander.Command.DropBoxUploader.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.DropBoxUploader.Command
{
	public abstract class ExecuteDropBoxUploaderWorkflowMessageHandlerBase
	{
	    private DropBoxConfiguration GetDropBoxConfiguration()
		{
			var config = DropBoxConfiguration.GetStandardConfiguration();
		    config.APIVersion = DropBoxAPIVersion.V1;
			config.AuthorizationCallBack = new Uri("http://www.talifun.com/oauth/dropbox.htm");
			return config;
		}

	    private ICloudStorageAccessToken GetAccessToken(DropBoxConfiguration config, IAuthenticationSettings authenticationSettings)
        {
            ICloudStorageAccessToken accessToken = null;
            if (!string.IsNullOrEmpty(authenticationSettings.DropBoxAuthenticationSecret))
            {
                accessToken = DropBoxExtensions.GetDropBoxAccessToken(authenticationSettings.DropBoxAuthenticationKey, authenticationSettings.DropBoxAuthenticationSecret, authenticationSettings.DropBoxApiKey, authenticationSettings.DropBoxApiSecret);
            }
            else
            {
                var requestToken = DropBoxExtensions.GetDropBoxRequestToken(authenticationSettings.DropBoxRequestKey, authenticationSettings.DropBoxRequestSecret);
                accessToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(config, authenticationSettings.DropBoxApiKey, authenticationSettings.DropBoxApiSecret, requestToken);

                authenticationSettings.DropBoxAuthenticationKey = accessToken.GetTokenKey();
                authenticationSettings.DropBoxAuthenticationSecret = accessToken.GetTokenSecret();
            }

            return accessToken;
        }

        protected void ExecuteUpload(IExecuteDropBoxUploaderWorkflowMessage message, FileInfo inputFilePath)
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
                var accessToken = GetAccessToken(configuration, message.Settings.Authentication);
				var storage = new CloudStorage();
				storage.Open(configuration, accessToken);

				var folder = string.IsNullOrEmpty(message.Settings.Folder) ? storage.GetRoot() : storage.GetFolder(message.Settings.Folder);
				if (folder == null)
				{
					throw new Exception(string.Format("Folder does not exist - {0}", message.Settings.Folder));
				}

			    var file = storage.CreateFile(folder, inputFilePath.Name);
			    var uploader = file.GetDataTransferAccessor();
			    using (var inputFileStream = inputFilePath.OpenRead())
			    {
			        uploader.Transfer(inputFileStream, nTransferDirection.nUpload, FileOperationProgressChanged, task);
			    }

			    if (storage.IsOpened)
				{
					storage.Close();
				}
			}
			, cancellationToken)
			.ContinueWith((t) =>
			{
				var executedDropBoxUploaderWorkflowMessage = new ExecutedDropBoxUploaderWorkflowMessage
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
