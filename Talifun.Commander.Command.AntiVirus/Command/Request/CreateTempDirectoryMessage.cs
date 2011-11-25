using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Request
{
	public class CreateTempDirectoryMessage : CorrelatedMessageBase<CreateTempDirectoryMessage>
	{
		public string FilePath { get; set; }
		public string InputFilePath { get; set; }
	}
}
