using System;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Response
{
	public interface IMovedProcessedFileIntoErrorDirectoryMessage : ICommandIdentifier
	{
		Guid CorrelationId { get;  set; }
		string OutputFilePath { get; set; }
	}
}