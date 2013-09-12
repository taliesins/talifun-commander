using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Response
{
	public interface IMovedProcessedFileIntoErrorDirectoryMessage : ICommandIdentifier
	{
		string OutputFilePath { get; set; }
	}
}