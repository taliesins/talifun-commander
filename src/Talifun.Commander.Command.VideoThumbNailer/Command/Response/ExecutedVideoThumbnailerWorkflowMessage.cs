using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.VideoThumbNailer.Command.Response
{
	public class ExecutedVideoThumbnailerWorkflowMessage : CorrelatedMessageBase<ExecutedVideoThumbnailerWorkflowMessage>, IExecutedVideoThumbnailerWorkflowMessage
	{
		public bool ThumbnailCreationSuccessful { get; set; }
		public string OutPutFilePath { get; set; }
		public string Output { get; set; }
	}
}
