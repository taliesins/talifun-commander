using MassTransit;
using MassTransit.Distributor;
using MassTransit.Saga;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command
{
	public class CommandManagerServiceBuses : ICommandManagerServiceBuses
	{
		public const string CommandManagerBusName = "CommandManager";
		public const string ConfigurationCheckerBusName = "ConfigurationChecker1";
		private ISagaRepository<TestConfigurationSaga> _testConfigurationSagaRepository;

		public void Start()
		{
			_testConfigurationSagaRepository = SetupSagaRepository<TestConfigurationSaga>();

			BusDriver.Instance.AddBus(ConfigurationCheckerBusName, string.Format("loopback://localhost/{0}", ConfigurationCheckerBusName), x =>
			{
				x.ImplementSagaDistributorWorker(_testConfigurationSagaRepository);
			});
	
			BusDriver.Instance.AddBus(CommandManagerBusName, string.Format("loopback://localhost/{0}", CommandManagerBusName), x =>
			{
				x.SetConcurrentConsumerLimit(4);
				x.UseSagaDistributorFor<TestConfigurationSaga>();
			});
		}

		public void Stop()
		{
			BusDriver.Instance.RemoveBus(ConfigurationCheckerBusName);
			BusDriver.Instance.RemoveBus(CommandManagerBusName);
			_testConfigurationSagaRepository = null;
		}

		private static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>() where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();
			return sagaRepository;
		}
	}
}
