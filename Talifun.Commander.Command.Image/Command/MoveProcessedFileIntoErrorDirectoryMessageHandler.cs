using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.Command.Response;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.Image.Command
{
	public class MoveProcessedFileIntoErrorDirectoryMessageHandler : MoveProcessedFileIntoErrorDirectoryMessageHandlerBase<MoveProcessedFileIntoErrorDirectoryMessage, MovedProcessedFileIntoErrorDirectoryMessage>
	{
		protected override void PublishMessage(MovedProcessedFileIntoErrorDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(ImageConversionService.BusName);
			bus.Publish(message);
		}
	}
}