using MassTransit;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.Command.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.Audio.Command
{
	public class MoveProcessedFileIntoErrorDirectoryMessageHandler : MoveProcessedFileIntoErrorDirectoryMessageHandlerBase<MoveProcessedFileIntoErrorDirectoryMessage, MovedProcessedFileIntoErrorDirectoryMessage>
	{
		protected override void PublishMessage(MovedProcessedFileIntoErrorDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(AudioConversionService.BusName);
			bus.Publish(message);
		}
	}
}