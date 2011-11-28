using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	public interface IMoveProcessedFileIntoOutputDirectoryMessage : ICommandIdentifier
	{
		string OutPutPath { get; set; }
		string OutputFilePath { get; set; }
	}
}
