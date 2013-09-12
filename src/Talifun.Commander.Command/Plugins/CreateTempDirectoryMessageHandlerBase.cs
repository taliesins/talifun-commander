using System.IO;
using MassTransit;
using Talifun.Commander.Command.Plugins.Request;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.Plugins
{
	public abstract class CreateTempDirectoryMessageHandlerBase<TRequest, TResponse> : Consumes<TRequest>.All
		where TRequest : class, ICreateTempDirectoryMessage
		where TResponse : class, ICreatedTempDirectoryMessage, new()
	{
		public void Consume(TRequest message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(message.Prefix, message.WorkingDirectoryPath, message.CorrelationId.ToString());

			workingDirectoryPath.Create();

			var createdTempDirectoryMessage = new TResponse()
			{
				CorrelationId = message.CorrelationId,
				WorkingDirectoryPath = workingDirectoryPath.FullName
			};

			PublishMessage(createdTempDirectoryMessage);
		}

		protected abstract void PublishMessage(TResponse message);
	}
}
