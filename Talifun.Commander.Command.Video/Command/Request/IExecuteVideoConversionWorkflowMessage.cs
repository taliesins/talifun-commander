using System.Collections.Generic;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.Containers;

namespace Talifun.Commander.Command.Video.Command.Request
{
	public interface IExecuteVideoConversionWorkflowMessage : ICommandIdentifier
	{
		IContainerSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
		string WorkingDirectoryPath { get; set; }
	}
}
