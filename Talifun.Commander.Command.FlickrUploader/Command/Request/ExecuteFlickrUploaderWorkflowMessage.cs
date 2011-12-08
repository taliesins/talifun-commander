using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Settings;

namespace Talifun.Commander.Command.FlickrUploader.Command.Request
{
	public class ExecuteFlickrUploaderWorkflowMessage : CorrelatedMessageBase<ExecuteFlickrUploaderWorkflowMessage>, IExecuteFlickrUploaderWorkflowMessage
	{
		public IFlickrUploaderSettings Settings { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
		public string InputFilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
