using System.IO;
using MassTransit;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.Command.Response;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	public class DeleteTempDirectoryMessageHandler : Consumes<DeleteTempDirectoryMessage>.All
	{
		public void Consume(DeleteTempDirectoryMessage message)
		{
			var workingDirectoryPath = new DirectoryInfo(message.WorkingPath);
			if (workingDirectoryPath.Exists)
			{
				workingDirectoryPath.Delete(true);
			}

			var deletedTempDirectoryMessage = new DeletedTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(AntiVirusService.BusName);
			bus.Publish(deletedTempDirectoryMessage);
		}
	}
}
