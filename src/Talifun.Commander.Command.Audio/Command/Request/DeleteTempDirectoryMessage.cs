using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Request;

namespace Talifun.Commander.Command.Audio.Command.Request
{
	public class DeleteTempDirectoryMessage : CorrelatedMessageBase<DeleteTempDirectoryMessage>, IDeleteTempDirectoryMessage
	{
		public string WorkingPath { get; set; }
	}
}