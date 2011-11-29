using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.Command.Response;

namespace Talifun.Commander.Command.Video.Command
{
	public class CreateTempDirectoryMessageHandler : CreateTempDirectoryMessageHandlerBase<CreateTempDirectoryMessage, CreatedTempDirectoryMessage>
	{
		protected override void PublishMessage(CreatedTempDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(VideoConversionService.BusName);
			bus.Publish(message);
		}
	}
}