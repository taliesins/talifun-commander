using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command.Request
{
	public class DeleteTempDirectoryMessage : CorrelatedMessageBase<DeleteTempDirectoryMessage>
	{
		public string WorkingPath { get; set; }
	}
}
