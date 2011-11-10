﻿using System;
using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher.Messages;

namespace Talifun.Commander.Command.FileMatcher.MessageHandlers
{
	public class CreateTempDirectoryMessageHandler : Consumes<CreateTempDirectoryMessage>.All
	{
		public void Consume(CreateTempDirectoryMessage message)
		{
			var fileInfo = new FileInfo(message.FilePath);
			if (!fileInfo.Exists)
			{
				return;
			}

			fileInfo.WaitForFileToUnlock(10, 500);

			var fileName = fileInfo.Name;
			var uniqueDirectoryName = "master." + fileName + "." + Guid.NewGuid();

			var workingDirectoryPath = !string.IsNullOrEmpty(message.WorkingPath) ?
				new DirectoryInfo(Path.Combine(message.WorkingPath, uniqueDirectoryName))
				: new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));

			workingDirectoryPath.Create();

			var workingFilePath = Path.Combine(workingDirectoryPath.FullName, fileName);

			var tempDirectoryCreatedMessage = new CreatedTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId,
				WorkingFilePath = workingFilePath
			};

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(tempDirectoryCreatedMessage, x=>{});
		}
	}
}
