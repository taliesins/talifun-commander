using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.Command.Response;

namespace Talifun.Commander.Command.Video.Command
{
	public class DeleteTempDirectoryMessageHandler : DeleteTempDirectoryMessageHandlerBase<DeleteTempDirectoryMessage, DeletedTempDirectoryMessage>
	{
		protected override void PublishMessage(DeletedTempDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(VideoConversionService.BusName);
			bus.Publish(message);
		}
	}
}
