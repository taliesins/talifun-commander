using System.Collections.Generic;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Request
{
	public interface IExecuteAntiVirusWorkflowMessage : ICommandIdentifier
	{
		IAntiVirusSettings Settings { get; set; }
		IDictionary<string, string> AppSettings { get; set; }
		string InputFilePath { get; set; }
		string OutputDirectoryPath { get; set; }
	}
}
