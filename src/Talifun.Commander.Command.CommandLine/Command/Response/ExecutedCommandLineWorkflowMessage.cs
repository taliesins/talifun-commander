using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.CommandLine.Command.Response
{
	public class ExecutedCommandLineWorkflowMessage : CorrelatedMessageBase<ExecutedCommandLineWorkflowMessage>, IExecutedCommandLineWorkflowMessage
	{
		public bool EncodeSuccessful { get; set; }
		public string OutPutFilePath { get; set; }
		public string Output { get; set; }
	}
}
