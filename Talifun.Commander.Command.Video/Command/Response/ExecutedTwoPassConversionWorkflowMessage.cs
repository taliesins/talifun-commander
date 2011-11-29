using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Video.Command.Response
{
	public class ExecutedTwoPassConversionWorkflowMessage : CorrelatedMessageBase<ExecutedTwoPassConversionWorkflowMessage>, IExecutedVideoConversionWorkflowMessage
	{
		public bool EncodeSuccessful { get; set; }
		public string OutPutFilePath { get; set; }
		public string Output { get; set; }
	}
}