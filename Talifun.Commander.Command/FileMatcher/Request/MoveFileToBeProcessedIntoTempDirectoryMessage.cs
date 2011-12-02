using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Request
{
	public class MoveFileToBeProcessedIntoTempDirectoryMessage : CorrelatedMessageBase<MoveFileToBeProcessedIntoTempDirectoryMessage>
	{
		public string FilePath { get; set; }
		public string WorkingDirectoryPath { get; set; }
	}
}
