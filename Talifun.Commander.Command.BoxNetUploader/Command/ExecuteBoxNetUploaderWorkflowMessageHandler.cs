using System.IO;
using MassTransit;
using Talifun.Commander.Command.BoxNetUploader.Command.Request;

namespace Talifun.Commander.Command.BoxNetUploader.Command
{
	public class ExecuteBoxNetUploaderWorkflowMessageHandler : ExecuteBoxNetUploaderWorkflowMessageHandlerBase, Consumes<ExecuteBoxNetUploaderWorkflowMessage>.All
	{
		public void Consume(ExecuteBoxNetUploaderWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);
			var authenticationToken = CheckAuthenticationToken(message.Settings.Authentication);
			ExecuteUpload(message, authenticationToken, inputFilePath);
		}
	}
}
