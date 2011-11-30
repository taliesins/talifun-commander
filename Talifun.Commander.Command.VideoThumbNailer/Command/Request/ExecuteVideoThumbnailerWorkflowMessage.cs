using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.VideoThumbnailer;
using Talifun.Commander.Command.VideoThumbnailer.Command.ThumbnailSettings;

namespace Talifun.Commander.Command.VideoThumbNailer.Command.Request
{
	public class ExecuteVideoThumbnailerWorkflowMessage : CorrelatedMessageBase<ExecuteVideoThumbnailerWorkflowMessage>, IExecuteVideoThumbnailerWorkflowMessage
	{
		public IThumbnailerSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
