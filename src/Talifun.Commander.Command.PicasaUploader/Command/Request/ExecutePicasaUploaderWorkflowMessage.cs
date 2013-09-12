using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command.Settings;

namespace Talifun.Commander.Command.PicasaUploader.Command.Request
{
	public class ExecutePicasaUploaderWorkflowMessage : CorrelatedMessageBase<ExecutePicasaUploaderWorkflowMessage>, IExecutePicasaUploaderWorkflowMessage
	{
		public IPicasaUploaderSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
