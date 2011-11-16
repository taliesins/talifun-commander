using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Request
{
	public class MoveProcessedFileIntoCompletedDirectoryMessage : CorrelatedMessageBase<MoveProcessedFileIntoCompletedDirectoryMessage>
	{
		public string WorkingFilePath { get; set; }
		public string CompletedPath { get; set; }
	}
}
