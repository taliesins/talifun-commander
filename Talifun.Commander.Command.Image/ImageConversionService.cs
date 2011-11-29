using MassTransit;
using Talifun.Commander.Command.Image.Command;
using Talifun.Commander.Command.Image.CommandTester;
using Talifun.Commander.Command.Image.Configuration;

namespace Talifun.Commander.Command.Image
{
	public class ImageConversionService : CommandServiceBase<ImageConversionSaga, ImageConversionConfigurationTesterSaga>
	{
		static ImageConversionService()
		{
			Settings = ImageConversionConfiguration.Instance;
		}

		public override void Configure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
			    subscriber.Consumer<CreateTempDirectoryMessageHandler>();
				subscriber.Consumer<ExecuteImageConversionWorkflowMessageHandler>();
				subscriber.Consumer<MoveProcessedFileIntoErrorDirectoryMessageHandler>();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>();
			});
		}
	}
}
