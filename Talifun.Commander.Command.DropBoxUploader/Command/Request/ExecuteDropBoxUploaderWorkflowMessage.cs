using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.DropBoxUploader.Command.Settings;

namespace Talifun.Commander.Command.DropBoxUploader.Command.Request
{
	public class ExecuteDropBoxUploaderWorkflowMessage : CorrelatedMessageBase<ExecuteDropBoxUploaderWorkflowMessage>, IExecuteDropBoxUploaderWorkflowMessage
	{
		public IDropBoxUploaderSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
