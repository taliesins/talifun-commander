using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher.Messages;

namespace Talifun.Commander.Command.FileMatcher.MessageHandlers
{
	public class DeleteTempDirectoryMessageHandler : Consumes<DeleteTempDirectoryMessage>.All
	{
		public void Consume(DeleteTempDirectoryMessage message)
		{
			var workingDirectoryPath = new FileInfo(message.WorkingFilePath).Directory;
			if (workingDirectoryPath.Exists)
			{
				workingDirectoryPath.Delete(true);
			}

			var deletedTempDirectoryMessage = new DeletedTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(deletedTempDirectoryMessage, x => { });
		}
	}
}
