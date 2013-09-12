using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Response
{
	public interface IMovedProcessedFileIntoOutputDirectoryMessage : ICommandIdentifier
	{
		string OutputFilePath { get; set; }
	}
}