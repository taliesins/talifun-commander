using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Request
{
	public class MoveProcessedFileIntoOutputDirectoryMessage : CorrelatedMessageBase<MoveProcessedFileIntoOutputDirectoryMessage>
	{
		public string OutPutPath { get; set; }
		public string OutputFilePath { get; set; }
	}
}
