using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.Command.Response;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.Image.Command
{
	public class CreateTempDirectoryMessageHandler : CreateTempDirectoryMessageHandlerBase<CreateTempDirectoryMessage, CreatedTempDirectoryMessage>
	{
		protected override void PublishMessage(CreatedTempDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(ImageConversionService.BusName);
			bus.Publish(message);
		}
	}
}