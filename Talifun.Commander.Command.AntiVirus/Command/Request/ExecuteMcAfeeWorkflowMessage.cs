using System.Collections.Generic;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Request
{
	public class ExecuteMcAfeeWorkflowMessage : CorrelatedMessageBase<ExecuteMcAfeeWorkflowMessage>, IExecuteAntiVirusWorkflowMessage
	{
		public IAntiVirusSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
