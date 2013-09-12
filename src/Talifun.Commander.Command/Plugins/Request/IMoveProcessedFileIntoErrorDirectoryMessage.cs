using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	public interface IMoveProcessedFileIntoErrorDirectoryMessage : ICommandIdentifier
	{
		string ErrorDirectoryPath { get; set; }
		string OutputFilePath { get; set; }
	}
}
