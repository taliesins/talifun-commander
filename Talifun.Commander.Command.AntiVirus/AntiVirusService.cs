using Talifun.Commander.Command.AntiVirus.Command;
using Talifun.Commander.Command.AntiVirus.CommandTester;
using Talifun.Commander.Command.AntiVirus.Configuration;
using MassTransit;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusService : CommandServiceBase<AntiVirusSaga, AntiVirusConfigurationTesterSaga>
	{
		static AntiVirusService()
		{
			Settings = AntiVirusConfiguration.Instance;
		}

		public override void Configure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteMcAfeeWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>().Permanent();
			});
		}
	}
}
