using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	public interface IMoveProcessedFileIntoOutputDirectoryMessage : ICommandIdentifier
	{
		string OutputDirectoryPath { get; set; }
		string OutputFilePath { get; set; }
	}
}
