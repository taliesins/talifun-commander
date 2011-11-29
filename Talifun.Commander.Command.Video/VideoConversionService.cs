using MassTransit;
using Talifun.Commander.Command.Video.Command;
using Talifun.Commander.Command.Video.CommandTester;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video
{
	public class VideoConversionService : CommandServiceBase<VideoConversionSaga, VideoConversionConfigurationTesterSaga>
	{
		static VideoConversionService()
		{
			Settings = VideoConversionConfiguration.Instance;
		}

		public override void Configure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
			    subscriber.Consumer<CreateTempDirectoryMessageHandler>();
				subscriber.Consumer<ExecuteFlvConversionWorkflowMessageHandler>();
				subscriber.Consumer<ExecuteMp4ConversionWorkflowMessageHandler>();
				subscriber.Consumer<ExecuteOnePassConversionWorkflowMessageHandler>();
				subscriber.Consumer<ExecuteTwoPassConversionWorkflowMessageHandler>();
				subscriber.Consumer<MoveProcessedFileIntoErrorDirectoryMessageHandler>();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>();
			});
		}
	}
}
