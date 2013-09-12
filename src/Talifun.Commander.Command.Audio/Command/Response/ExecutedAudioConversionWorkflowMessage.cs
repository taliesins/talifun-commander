using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command.Response
{
	public class ExecutedAudioConversionWorkflowMessage : CorrelatedMessageBase<ExecutedAudioConversionWorkflowMessage>, IExecutedAudioConversionWorkflowMessage
	{
		public bool EncodeSuccessful { get; set; }
		public string OutPutFilePath { get; set; }
		public string Output { get; set; }
	}
}
