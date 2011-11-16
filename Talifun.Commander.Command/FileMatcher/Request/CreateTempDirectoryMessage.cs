using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Request
{
	public class CreateTempDirectoryMessage : CorrelatedMessageBase<CreateTempDirectoryMessage>
	{
		public string FilePath { get; set; }
		public string WorkingPath { get; set; }
	}
}
