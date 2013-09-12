using MassTransit;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.Command.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.Audio.Command
{
	public class MoveProcessedFileIntoOutputDirectoryMessageHandler : MoveProcessedFileIntoOutputDirectoryMessageHandlerBase<MoveProcessedFileIntoOutputDirectoryMessage, MovedProcessedFileIntoOutputDirectoryMessage>
	{
		protected override void PublishMessage(MovedProcessedFileIntoOutputDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(AudioConversionService.BusName);
			bus.Publish(message);
		}
	}
}
