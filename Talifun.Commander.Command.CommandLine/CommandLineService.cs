using MassTransit;
using Talifun.Commander.Command.CommandLine.Command;
using Talifun.Commander.Command.CommandLine.CommandTester;
using Talifun.Commander.Command.CommandLine.Configuration;

namespace Talifun.Commander.Command.CommandLine
{
	public class CommandLineService : CommandServiceBase<CommandLineSaga, CommandLineConfigurationTesterSaga>
	{
		static CommandLineService()
		{
			Settings = CommandLineConfiguration.Instance;
		}

		public override void Configure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
			    subscriber.Consumer<CreateTempDirectoryMessageHandler>();
				subscriber.Consumer<ExecuteCommandLineWorkflowMessageHandler>();
				subscriber.Consumer<MoveProcessedFileIntoErrorDirectoryMessageHandler>();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>();
			});
		}
	}
}
