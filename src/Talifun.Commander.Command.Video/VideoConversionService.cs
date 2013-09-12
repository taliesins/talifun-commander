using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.CommandTester;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video
{
	public class VideoConversionService : CommandServiceBase<VideoConversionSaga, VideoConversionConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteVideoConversionWorkflowMessage, CancellableTask> CommandLineExecutors { get; set; }

		static VideoConversionService()
		{
			Settings = VideoConversionConfiguration.Instance;
			CommandLineExecutors = new ConcurrentDictionary<IExecuteVideoConversionWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<RetrieveMetaDataMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteFlvConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteMp4ConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteOnePassConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteTwoPassConversionWorkflowMessageHandler>().Permanent();
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
