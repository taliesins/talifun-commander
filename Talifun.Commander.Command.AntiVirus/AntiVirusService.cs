using Talifun.Commander.Command.AntiVirus.Command;
using Talifun.Commander.Command.AntiVirus.CommandTester;
using Talifun.Commander.Command.AntiVirus.Configuration;
using MassTransit;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusService : CommandServiceBase<AntiVirusSaga, AntiVirusTesterSaga>
	{
		static AntiVirusService()
		{
			Settings = AntiVirusConfiguration.Instance;
		}

		public override void Configure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
			    subscriber.Consumer<CreateTempDirectoryMessageHandler>();
				subscriber.Consumer<ExecuteMcAfeeWorkflowMessageHandler>();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>();
			});
		}
	}
}
