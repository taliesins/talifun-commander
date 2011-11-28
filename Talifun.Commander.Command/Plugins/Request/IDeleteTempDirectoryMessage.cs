using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	public interface IDeleteTempDirectoryMessage : ICommandIdentifier
	{
		Guid CorrelationId { get;  set; }
		string WorkingPath { get; set; }
	}
}
