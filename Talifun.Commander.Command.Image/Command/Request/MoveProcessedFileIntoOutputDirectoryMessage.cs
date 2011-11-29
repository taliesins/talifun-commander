using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Request;

namespace Talifun.Commander.Command.Image.Command.Request
{
	public class MoveProcessedFileIntoOutputDirectoryMessage : CorrelatedMessageBase<MoveProcessedFileIntoOutputDirectoryMessage>, IMoveProcessedFileIntoOutputDirectoryMessage
	{
		public string OutputDirectoryPath { get; set; }
		public string OutputFilePath { get; set; }
	}
}
