using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Response
{
	public interface IDeletedTempDirectoryMessage : ICommandIdentifier
	{
		Guid CorrelationId { get;  set; }
	}
}