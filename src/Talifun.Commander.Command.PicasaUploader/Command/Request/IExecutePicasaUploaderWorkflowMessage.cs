using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command.Settings;

namespace Talifun.Commander.Command.PicasaUploader.Command.Request
{
	public interface IExecutePicasaUploaderWorkflowMessage : ICommandIdentifier
	{
		IPicasaUploaderSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
	}
}
