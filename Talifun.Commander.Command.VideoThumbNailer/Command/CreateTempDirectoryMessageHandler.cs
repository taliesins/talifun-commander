using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;
using Talifun.Commander.Command.VideoThumbNailer.Command.Request;
using Talifun.Commander.Command.VideoThumbNailer.Command.Response;
using Talifun.Commander.Command.VideoThumbnailer;

namespace Talifun.Commander.Command.VideoThumbNailer.Command
{
	public class CreateTempDirectoryMessageHandler : CreateTempDirectoryMessageHandlerBase<CreateTempDirectoryMessage, CreatedTempDirectoryMessage>
	{
		protected override void PublishMessage(CreatedTempDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(VideoThumbnailerService.BusName);
			bus.Publish(message);
		}
	}
}