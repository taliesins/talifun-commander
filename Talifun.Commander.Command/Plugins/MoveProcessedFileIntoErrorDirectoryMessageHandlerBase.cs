﻿using System.IO;
using MassTransit;
using Talifun.Commander.Command.Plugins.Request;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.Plugins
{
	public abstract class MoveProcessedFileIntoErrorDirectoryMessageHandlerBase<TRequest, TResponse> : Consumes<TRequest>.All
		where TRequest : class, IMoveProcessedFileIntoErrorDirectoryMessage
		where TResponse : class, IMovedProcessedFileIntoErrorDirectoryMessage, new()
	{
		public void Consume(TRequest message)
		{
			var inputFilePath = new FileInfo(message.OutputFilePath);
			inputFilePath.WaitForFileToUnlock(10, 500);
			inputFilePath.Refresh();

			var outputFilePath = new FileInfo(Path.Combine(message.ErrorDirectoryPath, inputFilePath.Name));

			if (outputFilePath.Exists)
			{
				outputFilePath.Delete();
			}

			//Make sure that processing on file has stopped
			inputFilePath.WaitForFileToUnlock(10, 500);
			inputFilePath.Refresh();

			inputFilePath.MoveTo(outputFilePath.FullName);

			var movedProcessedFileIntoOutputDirectoryMessage = new TResponse()
			{
				CorrelationId = message.CorrelationId,
				OutputFilePath = outputFilePath.FullName
			};

			PublishMessage(movedProcessedFileIntoOutputDirectoryMessage);
		}

		protected abstract void PublishMessage(TResponse message);
	}
}
