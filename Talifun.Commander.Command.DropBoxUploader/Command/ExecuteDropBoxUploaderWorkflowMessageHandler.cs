using System.IO;
using MassTransit;
using Talifun.Commander.Command.DropBoxUploader.Command.Request;

namespace Talifun.Commander.Command.DropBoxUploader.Command
{
	public class ExecuteDropBoxUploaderWorkflowMessageHandler : ExecuteDropBoxUploaderWorkflowMessageHandlerBase, Consumes<ExecuteDropBoxUploaderWorkflowMessage>.All
	{
		public void Consume(ExecuteDropBoxUploaderWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);
			var authenticationToken = CheckAuthenticationToken(message.Settings.Authentication);
			ExecuteUpload(message, authenticationToken, inputFilePath);
		}
	}
}
