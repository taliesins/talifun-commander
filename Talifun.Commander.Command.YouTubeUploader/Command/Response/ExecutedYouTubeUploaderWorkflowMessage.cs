using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Response
{
	public class ExecutedYouTubeUploaderWorkflowMessage : CorrelatedMessageBase<ExecutedYouTubeUploaderWorkflowMessage>, IExecutedYouTubeUploaderWorkflowMessage
	{
		public Exception Error { get; set; }
		public bool Cancelled { get; set; }
		public string VideoId { get; set; }
	}
}
