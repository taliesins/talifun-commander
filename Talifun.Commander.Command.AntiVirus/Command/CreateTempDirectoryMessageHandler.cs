using System.IO;
using MassTransit;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.Command.Response;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	public class CreateTempDirectoryMessageHandler : Consumes<CreateTempDirectoryMessage>.All
	{
		public void Consume(CreateTempDirectoryMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(message.Prefix, message.WorkingDirectoryPath, message.CorrelationId.ToString());

			workingDirectoryPath.Create();

			var createdTempDirectoryMessage = new CreatedTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId,
				WorkingPath = workingDirectoryPath.FullName
			};

			var bus = BusDriver.Instance.GetBus(AntiVirusService.BusName);
			bus.Publish(createdTempDirectoryMessage);
		}
	}
}
