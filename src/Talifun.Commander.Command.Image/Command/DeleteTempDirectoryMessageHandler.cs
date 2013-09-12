using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.Command.Response;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.Image.Command
{
	public class DeleteTempDirectoryMessageHandler : DeleteTempDirectoryMessageHandlerBase<DeleteTempDirectoryMessage, DeletedTempDirectoryMessage>
	{
		protected override void PublishMessage(DeletedTempDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(ImageConversionService.BusName);
			bus.Publish(message);
		}
	}
}
