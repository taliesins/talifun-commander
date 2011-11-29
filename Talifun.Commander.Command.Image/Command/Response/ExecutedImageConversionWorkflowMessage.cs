using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Image.Command.Response
{
	public class ExecutedImageConversionWorkflowMessage : CorrelatedMessageBase<ExecutedImageConversionWorkflowMessage>, IExecutedImageConversionWorkflowMessage
	{
		public bool EncodeSuccessful { get; set; }
		public string OutPutFilePath { get; set; }
		public string Output { get; set; }
	}
}
