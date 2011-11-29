using MassTransit;
using Talifun.Commander.Command.CommandLine.Command.Request;
using Talifun.Commander.Command.CommandLine.Command.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Plugins;

namespace Talifun.Commander.Command.CommandLine.Command
{
	public class MoveProcessedFileIntoOutputDirectoryMessageHandler : MoveProcessedFileIntoOutputDirectoryMessageHandlerBase<MoveProcessedFileIntoOutputDirectoryMessage, MovedProcessedFileIntoOutputDirectoryMessage>
	{
		protected override void PublishMessage(MovedProcessedFileIntoOutputDirectoryMessage message)
		{
			var bus = BusDriver.Instance.GetBus(CommandLineService.BusName);
			bus.Publish(message);
		}
	}
}
