using MassTransit;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.Command.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.Audio.Command
{
	public class DeleteTempDirectoryMessageHandler : DeleteTempDirectoryMessageHandlerBase<DeleteTempDirectoryMessage, DeletedTempDirectoryMessage>
	{
		protected override void PublishMessage(DeletedTempDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(AudioConversionService.BusName);
			bus.Publish(message);
		}
	}
}
