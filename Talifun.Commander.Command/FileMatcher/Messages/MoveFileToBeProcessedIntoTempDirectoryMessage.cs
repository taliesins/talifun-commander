using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Messages
{
	public class MoveFileToBeProcessedIntoTempDirectoryMessage : CorrelatedMessageBase<MoveFileToBeProcessedIntoTempDirectoryMessage>
	{
		public string FilePath { get; set; }
		public string WorkingFilePath { get; set; }
	}
}
