using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FlickrUploader.Command.Response
{
	public class ExecutedFlickrUploaderWorkflowMessage : CorrelatedMessageBase<ExecutedFlickrUploaderWorkflowMessage>, IExecutedFlickrUploaderWorkflowMessage
	{
		public Exception Error { get; set; }
		public bool Cancelled { get; set; }
	}
}