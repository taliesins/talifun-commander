using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	public interface ICreateTempDirectoryMessage : ICommandIdentifier
	{
		Guid CorrelationId { get;  set; }
		string Prefix { get; set; }
		string InputFilePath { get; set; }
		string WorkingDirectoryPath { get; set; }
	}
}
