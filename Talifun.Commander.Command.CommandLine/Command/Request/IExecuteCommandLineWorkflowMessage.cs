using System;
using System.Collections.Generic;
using Talifun.Commander.Command.CommandLine.Command.Arguments;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.CommandLine.Command.Request
{
	public interface IExecuteCommandLineWorkflowMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		ICommandLineParameters Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
		string WorkingDirectoryPath { get; set; }
	}
}
