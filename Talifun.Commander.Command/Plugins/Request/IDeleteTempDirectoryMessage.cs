using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	public interface IDeleteTempDirectoryMessage : ICommandIdentifier
	{
		string WorkingPath { get; set; }
	}
}
