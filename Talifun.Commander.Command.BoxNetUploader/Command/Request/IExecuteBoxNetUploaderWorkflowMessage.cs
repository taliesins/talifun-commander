using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.BoxNetUploader.Command.Settings;

namespace Talifun.Commander.Command.BoxNetUploader.Command.Request
{
	public interface IExecuteBoxNetUploaderWorkflowMessage : ICommandIdentifier
	{
		IBoxNetUploaderSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
	}
}
