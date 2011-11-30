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
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteFlvConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteMp4ConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteOnePassConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteTwoPassConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoErrorDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>().Permanent();
			});
		}
	}
}
