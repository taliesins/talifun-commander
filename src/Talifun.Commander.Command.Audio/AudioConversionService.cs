using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Audio.Command;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.CommandTester;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio
{
	public class AudioConversionService : CommandServiceBase<AudioConversionSaga, AudioConversionConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteAudioConversionWorkflowMessage, CancellableTask> CommandLineExecutors { get; set; }

		static AudioConversionService()
		{
			Settings = AudioConversionConfiguration.Instance;
			CommandLineExecutors = new ConcurrentDictionary<IExecuteAudioConversionWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<RetrieveMetaDataMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteAudioConversionWorkflowMessageHandler>().Permanent();
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
