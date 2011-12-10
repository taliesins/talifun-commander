using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.DropBoxUploader.Command.Response
{
	public class ExecutedDropBoxUploaderWorkflowMessage : CorrelatedMessageBase<ExecutedDropBoxUploaderWorkflowMessage>, IExecutedDropBoxUploaderWorkflowMessage
	{
		public Exception Error { get; set; }
		public bool Cancelled { get; set; }
	}
}