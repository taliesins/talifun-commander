using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher.Request;
using Talifun.Commander.Command.FileMatcher.Response;

namespace Talifun.Commander.Command.FileMatcher
{
	public class MoveFileToBeProcessedIntoTempDirectoryMessageHandler : Consumes<MoveFileToBeProcessedIntoTempDirectoryMessage>.All
	{
		public void Consume(MoveFileToBeProcessedIntoTempDirectoryMessage message)
		{
			var inputFilePath = new FileInfo(message.FilePath);
			inputFilePath.WaitForFileToUnlock(10, 500);
			inputFilePath.Refresh();

			var outputFilePath = new FileInfo(message.WorkingFilePath);
			if (outputFilePath.Exists)
			{
				outputFilePath.Delete();
			}

			inputFilePath.MoveTo(outputFilePath.FullName);

			var movedFileToBeProcessedIntoTempDirectoryMessage = new MovedFileToBeProcessedIntoTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(movedFileToBeProcessedIntoTempDirectoryMessage);
		}
	}
}
