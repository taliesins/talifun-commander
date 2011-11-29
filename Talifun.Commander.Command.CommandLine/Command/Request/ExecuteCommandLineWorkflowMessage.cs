using System.Collections.Generic;
using Talifun.Commander.Command.CommandLine.Command.Arguments;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.CommandLine.Command.Request
{
	public class ExecuteCommandLineWorkflowMessage : CorrelatedMessageBase<ExecuteCommandLineWorkflowMessage>, IExecuteCommandLineWorkflowMessage
	{
		public ICommandLineParameters Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
