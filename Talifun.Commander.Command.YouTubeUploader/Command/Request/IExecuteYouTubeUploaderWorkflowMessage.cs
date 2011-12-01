using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.YouTubeUploader.Command.Settings;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Request
{
	public interface IExecuteYouTubeUploaderWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		IYouTubeUploaderSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
	}
}
