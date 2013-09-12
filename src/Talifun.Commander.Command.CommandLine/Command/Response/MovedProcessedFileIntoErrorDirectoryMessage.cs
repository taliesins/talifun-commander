using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.CommandLine.Command.Response
{
	public class MovedProcessedFileIntoErrorDirectoryMessage : CorrelatedMessageBase<MovedProcessedFileIntoErrorDirectoryMessage>, IMovedProcessedFileIntoErrorDirectoryMessage
	{
		public string OutputFilePath { get; set; }
	}
}
