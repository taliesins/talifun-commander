﻿using System.IO;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher.Request;
using Talifun.Commander.Command.FileMatcher.Response;

namespace Talifun.Commander.Command.FileMatcher
{
	public class DeleteTempDirectoryMessageHandler : Consumes<DeleteTempDirectoryMessage>.All
	{
		public void Consume(DeleteTempDirectoryMessage message)
		{
			var workingDirectoryPath = new FileInfo(message.WorkingFilePath).Directory;
			if (workingDirectoryPath.Exists)
			{
				workingDirectoryPath.Delete(true);
			}

			var deletedTempDirectoryMessage = new DeletedTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(deletedTempDirectoryMessage);
		}
	}
}
