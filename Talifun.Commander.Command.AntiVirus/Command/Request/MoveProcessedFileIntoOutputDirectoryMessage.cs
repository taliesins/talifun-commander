using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Request
{
	public class MoveProcessedFileIntoOutputDirectoryMessage : CorrelatedMessageBase<MoveProcessedFileIntoOutputDirectoryMessage>
	{
		public string WorkingDirectoryPath { get; set; }
		public string OutputPath { get; set; }
	}
}
