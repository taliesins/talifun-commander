using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.Containers;

namespace Talifun.Commander.Command.Video.Command.Request
{
	public class ExecuteMp4ConversionWorkflowMessage : CorrelatedMessageBase<ExecuteMp4ConversionWorkflowMessage>, IExecuteVideoConversionWorkflowMessage
	{
		public IContainerSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
