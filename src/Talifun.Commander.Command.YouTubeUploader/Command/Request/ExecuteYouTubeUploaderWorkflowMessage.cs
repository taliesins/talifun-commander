using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.YouTubeUploader.Command.Settings;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Request
{
	public class ExecuteYouTubeUploaderWorkflowMessage : CorrelatedMessageBase<ExecuteYouTubeUploaderWorkflowMessage>, IExecuteYouTubeUploaderWorkflowMessage
	{
		public IYouTubeUploaderSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
