using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Request;

namespace Talifun.Commander.Command.AntiVirus.Command.Request
{
	public class MoveProcessedFileIntoOutputDirectoryMessage : CorrelatedMessageBase<MoveProcessedFileIntoOutputDirectoryMessage>, IMoveProcessedFileIntoOutputDirectoryMessage
	{
		public string OutPutPath { get; set; }
		public string OutputFilePath { get; set; }
	}
}
