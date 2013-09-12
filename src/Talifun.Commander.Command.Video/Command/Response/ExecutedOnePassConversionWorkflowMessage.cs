using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Video.Command.Response
{
	public class ExecutedOnePassConversionWorkflowMessage : CorrelatedMessageBase<ExecutedOnePassConversionWorkflowMessage>, IExecutedVideoConversionWorkflowMessage
	{
		public bool EncodeSuccessful { get; set; }
		public string OutPutFilePath { get; set; }
		public string Output { get; set; }
	}
}
