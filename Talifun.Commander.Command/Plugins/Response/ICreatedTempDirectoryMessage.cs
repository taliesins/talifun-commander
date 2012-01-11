using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Response
{
	public interface ICreatedTempDirectoryMessage : ICommandIdentifier
	{
		string WorkingDirectoryPath { get; set; }
	}
}