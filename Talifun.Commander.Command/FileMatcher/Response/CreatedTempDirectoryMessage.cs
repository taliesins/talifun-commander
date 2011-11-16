using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Response
{
	public class CreatedTempDirectoryMessage : CorrelatedMessageBase<CreatedTempDirectoryMessage>
	{
		public string WorkingFilePath { get; set; }
	}
}
