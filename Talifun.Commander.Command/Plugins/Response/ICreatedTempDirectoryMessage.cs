using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Response
{
	public interface ICreatedTempDirectoryMessage : ICommandIdentifier
	{
		Guid CorrelationId { get;  set; }
		string WorkingDirectoryPath { get; set; }
	}
}