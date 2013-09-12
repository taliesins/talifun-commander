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
			var filesToMove = Directory.GetFiles(inputFilePath.DirectoryName, inputFilePath.Name + ".*");

			foreach (var fileNameToMove in filesToMove)
			{
				var fileToMove = new FileInfo(fileNameToMove);

				fileToMove.WaitForFileToUnlock(10, 500);
				fileToMove.Refresh();

				var outputFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileToMove.Name));
				if (outputFilePath.Exists)
				{
					outputFilePath.Delete();
				}

				fileToMove.MoveTo(outputFilePath.FullName);
			}

			var movedFileToBeProcessedIntoTempDirectoryMessage = new MovedFileToBeProcessedIntoTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(movedFileToBeProcessedIntoTempDirectoryMessage);
		}
	}
}
