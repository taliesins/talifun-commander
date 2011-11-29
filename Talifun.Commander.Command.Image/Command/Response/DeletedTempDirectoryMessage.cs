using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.Image.Command.Response
{
	public class DeletedTempDirectoryMessage : CorrelatedMessageBase<DeletedTempDirectoryMessage>, IDeletedTempDirectoryMessage
	{
	}
}
