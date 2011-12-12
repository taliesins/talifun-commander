using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BoxNet;
using Talifun.Commander.Command.BoxNetUploader.Command.Request;
using Talifun.Commander.Command.BoxNetUploader.Command.Response;
using Talifun.Commander.Command.BoxNetUploader.Command.Settings;
using Talifun.Commander.Command.Esb;
using MassTransit;

namespace Talifun.Commander.Command.BoxNetUploader.Command
{
	public abstract class ExecuteBoxNetUploaderWorkflowMessageHandlerBase
	{
		private BoxNetConfiguration GetBoxNetConfiguration()
		{
			var config = BoxNetConfiguration.GetBoxNetConfiguration();
			return config;
		}

		protected GenericNetworkCredentials CheckAuthenticationToken(IAuthenticationSettings authenticationSettings)
		{
			var credentials = new GenericNetworkCredentials
			{
			    UserName = authenticationSettings.BoxNetUsername, 
				Password = authenticationSettings.BoxNetPassword
			};

			return credentials;
		}

		protected void ExecuteUpload(IExecuteBoxNetUploaderWorkflowMessage message, GenericNetworkCredentials authenticationToken, FileInfo inputFilePath)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var task = Task.Factory.StartNew(() => { });
			task.ContinueWith((t) =>
			{
				BoxNetUploaderService.Uploaders.Add(message, new CancellableTask
				{
					Task = task,
					CancellationTokenSource = cancellationTokenSource
				});

				var configuration = GetBoxNetConfiguration();
				var storage = new CloudStorage();
				var accessToken = storage.Open(configuration, authenticationToken);

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
				var executedBoxNetUploaderWorkflowMessage = new ExecutedBoxNetUploaderWorkflowMessage()
				{
					CorrelationId = message.CorrelationId,
					Cancelled = t.IsCanceled,
					Error = t.Exception
				};

				var bus = BusDriver.Instance.GetBus(BoxNetUploaderService.BusName);
				bus.Publish(executedBoxNetUploaderWorkflowMessage);

				BoxNetUploaderService.Uploaders.Remove(message);
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
