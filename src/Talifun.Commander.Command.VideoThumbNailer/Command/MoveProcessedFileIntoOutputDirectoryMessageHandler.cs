using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;
using Talifun.Commander.Command.VideoThumbNailer.Command.Request;
using Talifun.Commander.Command.VideoThumbNailer.Command.Response;
using Talifun.Commander.Command.VideoThumbnailer;

namespace Talifun.Commander.Command.VideoThumbNailer.Command
{
	public class MoveProcessedFileIntoOutputDirectoryMessageHandler : MoveProcessedFileIntoOutputDirectoryMessageHandlerBase<MoveProcessedFileIntoOutputDirectoryMessage, MovedProcessedFileIntoOutputDirectoryMessage>
	{
		protected override void PublishMessage(MovedProcessedFileIntoOutputDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(VideoThumbnailerService.BusName);
			bus.Publish(message);
		}
	}
}
