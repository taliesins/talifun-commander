using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Messages
{
	public class CreatedTempDirectoryMessage : CorrelatedMessageBase<CreatedTempDirectoryMessage>
	{
		public string WorkingFilePath { get; set; }
	}
}
