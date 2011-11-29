using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.ImageSettings;

namespace Talifun.Commander.Command.Image.Command.Request
{
	public class ExecuteImageConversionWorkflowMessage : CorrelatedMessageBase<ExecuteImageConversionWorkflowMessage>, IExecuteImageConversionWorkflowMessage
	{
		public IImageResizeSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
