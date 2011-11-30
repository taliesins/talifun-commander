using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.VideoThumbnailer;
using Talifun.Commander.Command.VideoThumbnailer.Command.ThumbnailSettings;

namespace Talifun.Commander.Command.VideoThumbNailer.Command.Request
{
	public interface IExecuteVideoThumbnailerWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		IThumbnailerSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
		string WorkingDirectoryPath { get; set; }
	}
}
