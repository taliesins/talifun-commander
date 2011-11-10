using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Messages
{
	public class ProcessFileMatchesMessage : CorrelatedMessageBase<ProcessFileMatchesMessage>
	{
		public string WorkingFilePath { get; set; }
		public FileMatchElementCollection FileMatches { get; set; }
	}
}
