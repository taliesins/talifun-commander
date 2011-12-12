using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.BoxNetUploader.Command.Response
{
	public class ExecutedBoxNetUploaderWorkflowMessage : CorrelatedMessageBase<ExecutedBoxNetUploaderWorkflowMessage>, IExecutedBoxNetUploaderWorkflowMessage
	{
		public Exception Error { get; set; }
		public bool Cancelled { get; set; }
	}
}