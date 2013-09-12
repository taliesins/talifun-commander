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
			ExecuteUpload(message, inputFilePath);
		}
	}
}
