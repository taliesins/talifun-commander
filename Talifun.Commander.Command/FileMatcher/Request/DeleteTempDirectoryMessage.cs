using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Request
{
	public class DeleteTempDirectoryMessage : CorrelatedMessageBase<DeleteTempDirectoryMessage>
	{
		public string WorkingFilePath { get; set; }
	}
}
