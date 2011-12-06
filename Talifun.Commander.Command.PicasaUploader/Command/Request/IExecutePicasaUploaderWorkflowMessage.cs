using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command.Settings;

namespace Talifun.Commander.Command.PicasaUploader.Command.Request
{
	public interface IExecutePicasaUploaderWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		IPicasaUploaderSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
	}
}
