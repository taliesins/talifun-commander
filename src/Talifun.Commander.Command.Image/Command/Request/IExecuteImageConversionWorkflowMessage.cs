using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.ImageSettings;

namespace Talifun.Commander.Command.Image.Command.Request
{
	public interface IExecuteImageConversionWorkflowMessage : ICommandIdentifier
	{
		IImageResizeSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
		string WorkingDirectoryPath { get; set; }
	}
}
