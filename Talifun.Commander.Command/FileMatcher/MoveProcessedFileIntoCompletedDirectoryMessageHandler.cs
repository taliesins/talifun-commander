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
			var inputFilePath = new FileInfo(message.WorkingFilePath);
			if (!string.IsNullOrEmpty(message.CompletedPath) && inputFilePath.Exists)
			{
				var outputFilePath = new FileInfo(Path.Combine(message.CompletedPath, inputFilePath.Name));
				if (outputFilePath.Exists)
				{
					outputFilePath.Delete();
				}

				//Make sure that processing on file has stopped
				inputFilePath.WaitForFileToUnlock(10, 500);
				inputFilePath.Refresh();

				inputFilePath.MoveTo(outputFilePath.FullName);

				var supportFileNames = Directory.GetFiles(inputFilePath.DirectoryName, inputFilePath.Name + ".*");
				foreach (var supportFileName in supportFileNames)
				{
					var supportFile = new FileInfo(supportFileName);

					//Make sure that processing on file has stopped
					supportFile.WaitForFileToUnlock(10, 500);
					supportFile.Refresh();

					supportFile.MoveTo(Path.Combine(outputFilePath.DirectoryName, supportFile.Name));
				}
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
