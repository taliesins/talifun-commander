using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.PicasaUploader.Command.Response
{
	public class ExecutedPicasaUploaderWorkflowMessage : CorrelatedMessageBase<ExecutedPicasaUploaderWorkflowMessage>, IExecutedPicasaUploaderWorkflowMessage
	{
		public Exception Error { get; set; }
		public bool Cancelled { get; set; }
	}
}