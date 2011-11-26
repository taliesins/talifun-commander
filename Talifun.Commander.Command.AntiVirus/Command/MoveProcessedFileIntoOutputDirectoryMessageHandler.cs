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
			var inputFilePath = new FileInfo(message.OutputFilePath);
			inputFilePath.WaitForFileToUnlock(10, 500);
			inputFilePath.Refresh();

			var outputFilePath = new FileInfo(Path.Combine(message.OutPutPath, inputFilePath.Name));

			if (outputFilePath.Exists)
			{
				outputFilePath.Delete();
			}

			//Make sure that processing on file has stopped
			inputFilePath.WaitForFileToUnlock(10, 500);
			inputFilePath.Refresh();

			inputFilePath.MoveTo(outputFilePath.FullName);

			var movedProcessedFileIntoOutputDirectoryMessage = new MovedProcessedFileIntoOutputDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(AntiVirusService.BusName);
			bus.Publish(movedProcessedFileIntoOutputDirectoryMessage);
		}
	}
}
