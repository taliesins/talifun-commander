using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.BoxNetUploader.Command.Settings;

namespace Talifun.Commander.Command.BoxNetUploader.Command.Request
{
	public class ExecuteBoxNetUploaderWorkflowMessage : CorrelatedMessageBase<ExecuteBoxNetUploaderWorkflowMessage>, IExecuteBoxNetUploaderWorkflowMessage
	{
		public IBoxNetUploaderSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
