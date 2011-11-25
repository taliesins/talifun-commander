using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher.Request;
using Talifun.Commander.Command.FileMatcher.Response;

namespace Talifun.Commander.Command.FileMatcher
{
	public class MoveProcessedFileIntoCompletedDirectoryMessageHandler : Consumes<MoveProcessedFileIntoCompletedDirectoryMessage>.All
	{
		public void Consume(MoveProcessedFileIntoCompletedDirectoryMessage message)
		{
			var workingFilePath = new FileInfo(message.WorkingFilePath);
			if (!string.IsNullOrEmpty(message.CompletedPath) && workingFilePath.Exists)
			{
				var completedFilePath = new FileInfo(Path.Combine(message.CompletedPath, workingFilePath.Name));
				if (completedFilePath.Exists)
				{
					completedFilePath.Delete();
				}

				//Make sure that processing on file has stopped
				workingFilePath.WaitForFileToUnlock(10, 500);
				workingFilePath.Refresh();
				workingFilePath.MoveTo(completedFilePath.FullName);
			}

			var movedProcessedFileIntoCompletedDirectoryMessage = new MovedProcessedFileIntoCompletedDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(movedProcessedFileIntoCompletedDirectoryMessage);
		}
	}
}
