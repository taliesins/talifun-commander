using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.Command.Response;

namespace Talifun.Commander.Command.Video.Command
{
	public class MoveProcessedFileIntoErrorDirectoryMessageHandler : MoveProcessedFileIntoErrorDirectoryMessageHandlerBase<MoveProcessedFileIntoErrorDirectoryMessage, MovedProcessedFileIntoErrorDirectoryMessage>
	{
		protected override void PublishMessage(MovedProcessedFileIntoErrorDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(VideoConversionService.BusName);
			bus.Publish(message);
		}
	}
}