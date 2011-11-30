using MassTransit;
using Talifun.Commander.Command.VideoThumbNailer.Command;
using Talifun.Commander.Command.VideoThumbNailer.CommandTester;
using Talifun.Commander.Command.VideoThumbnailer.Configuration;

namespace Talifun.Commander.Command.VideoThumbnailer
{
	public class VideoThumbnailerService : CommandServiceBase<VideoThumbnailerSaga, VideoThumbnailerConfigurationTesterSaga>
	{
		static VideoThumbnailerService()
		{
			Settings = VideoThumbnailerConfiguration.Instance;
		}

		public override void Configure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteVideoThumbnailerWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoErrorDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>().Permanent();
			});
		}
	}
}
