using System.IO;
using MassTransit;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.Command.Response;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	public class MoveProcessedFileIntoOutputDirectoryMessageHandler : Consumes<MoveProcessedFileIntoOutputDirectoryMessage>.All
	{
		public void Consume(MoveProcessedFileIntoOutputDirectoryMessage message)
		{
			var outputPath = new FileInfo(message.OutputPath);
			outputPath.WaitForFileToUnlock(10, 500);
			outputPath.Refresh();
			outputPath.MoveTo(message.WorkingFilePath);

			var movedProcessedFileIntoOutputDirectoryMessage = new MovedProcessedFileIntoOutputDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(AntiVirusService.BusName);
			bus.Publish(movedProcessedFileIntoOutputDirectoryMessage);
		}
	}
}
