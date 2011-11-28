using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.Audio.Command.Response
{
	public class DeletedTempDirectoryMessage : CorrelatedMessageBase<DeletedTempDirectoryMessage>, IDeletedTempDirectoryMessage
	{
	}
}
