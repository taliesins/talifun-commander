using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Response
{
	public class ExecutedMcAfeeWorkflowMessage : CorrelatedMessageBase<ExecutedMcAfeeWorkflowMessage>, IExecutedAntiVirusWorkflowMessage
	{
		public bool FilePassed { get; set; }
		public string OutPutFilePath { get; set; }
		public string Output { get; set; }
	}
}
