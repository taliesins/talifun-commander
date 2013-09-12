using MassTransit;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.Command.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.Audio.Command
{
	public class CreateTempDirectoryMessageHandler : CreateTempDirectoryMessageHandlerBase<CreateTempDirectoryMessage, CreatedTempDirectoryMessage>
	{
		protected override void PublishMessage(CreatedTempDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(AudioConversionService.BusName);
			bus.Publish(message);
		}
	}
}