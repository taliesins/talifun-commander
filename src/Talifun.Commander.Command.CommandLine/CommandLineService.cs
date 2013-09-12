using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.CommandLine.Command;
using Talifun.Commander.Command.CommandLine.Command.Request;
using Talifun.Commander.Command.CommandLine.CommandTester;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.CommandLine
{
	public class CommandLineService : CommandServiceBase<CommandLineSaga, CommandLineConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteCommandLineWorkflowMessage, CancellableTask> CommandLineExecutors { get; set; }

		static CommandLineService()
		{
			Settings = CommandLineConfiguration.Instance;
			CommandLineExecutors = new ConcurrentDictionary<IExecuteCommandLineWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteCommandLineWorkflowMessageHandler>().Permanent();
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
