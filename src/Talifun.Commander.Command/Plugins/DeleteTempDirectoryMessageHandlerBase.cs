using System.IO;
using MassTransit;
using Talifun.Commander.Command.Plugins.Request;
using Talifun.Commander.Command.Plugins.Response;

namespace Talifun.Commander.Command.Plugins
{
	public abstract class DeleteTempDirectoryMessageHandlerBase<TRequest, TResponse> : Consumes<TRequest>.All
		where TRequest : class, IDeleteTempDirectoryMessage
		where TResponse : class, IDeletedTempDirectoryMessage, new()
	{
		public void Consume(TRequest message)
		{
			var workingDirectoryPath = new DirectoryInfo(message.WorkingPath);
			if (workingDirectoryPath.Exists)
			{
				workingDirectoryPath.Delete(true);
			}

			var deletedTempDirectoryMessage = new TResponse()
			{
				CorrelationId = message.CorrelationId
			};

			PublishMessage(deletedTempDirectoryMessage);
		}

		protected abstract void PublishMessage(TResponse message);
	}
}
