using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.VideoThumbNailer.Command;
using Talifun.Commander.Command.VideoThumbNailer.Command.Request;
using Talifun.Commander.Command.VideoThumbNailer.CommandTester;
using Talifun.Commander.Command.VideoThumbnailer.Configuration;

namespace Talifun.Commander.Command.VideoThumbnailer
{
	public class VideoThumbnailerService : CommandServiceBase<VideoThumbnailerSaga, VideoThumbnailerConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteVideoThumbnailerWorkflowMessage, CancellableTask> CommandLineExecutors { get; set; }

		static VideoThumbnailerService()
		{
			Settings = VideoThumbnailerConfiguration.Instance;
			CommandLineExecutors = new ConcurrentDictionary<IExecuteVideoThumbnailerWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
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

		public override void OnStop()
		{
			foreach (var commandLineExecutor in CommandLineExecutors)
			{
				commandLineExecutor.Value.CancellationTokenSource.Cancel();
			}
		}
	}
}
