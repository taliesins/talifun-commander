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
			var fileInfo = new FileInfo(message.FilePath);
			fileInfo.WaitForFileToUnlock(10, 500);
			fileInfo.Refresh();
			fileInfo.MoveTo(message.WorkingFilePath);

			var movedFileToBeProcessedIntoTempDirectoryMessage = new MovedFileToBeProcessedIntoTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(movedFileToBeProcessedIntoTempDirectoryMessage, x => { });
		}
	}
}
