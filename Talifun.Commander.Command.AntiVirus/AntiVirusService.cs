using System.Collections.Concurrent;
using System.Collections.Generic;
using Talifun.Commander.Command.AntiVirus.Command;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.CommandTester;
using Talifun.Commander.Command.AntiVirus.Configuration;
using MassTransit;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusService : CommandServiceBase<AntiVirusSaga, AntiVirusConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteAntiVirusWorkflowMessage, CancellableTask> CommandLineExecutors { get; private set; }

		static AntiVirusService()
		{
			Settings = AntiVirusConfiguration.Instance;
			CommandLineExecutors = new ConcurrentDictionary<IExecuteAntiVirusWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteMcAfeeWorkflowMessageHandler>().Permanent();
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
