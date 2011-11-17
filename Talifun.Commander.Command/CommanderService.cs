using System.ComponentModel.Composition.Hosting;
using MassTransit;
using MassTransit.Distributor;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Esb.Request;
using Talifun.Commander.Command.Esb.Response;
using Talifun.Commander.Command.FileMatcher;

namespace Talifun.Commander.Command
{
	public class CommanderService
	{
		public const string CommandManagerBusName = "CommandManager";
		public const string ConfigurationCheckerBusName = "ConfigurationChecker1";
		public const string FileMatcherBusName = "FileMatcher1";
		private ISagaRepository<TestConfigurationSaga> _testConfigurationSagaRepository;
		private ISagaRepository<FileMatcherSaga> _fileMatcherSagaRepository;

		public void Start()
		{
			_testConfigurationSagaRepository = SetupSagaRepository<TestConfigurationSaga>();
			_fileMatcherSagaRepository = SetupSagaRepository<FileMatcherSaga>();

			foreach (var commandService in Container.GetExportedValues<ICommandService>())
			{
				commandService.Start();
			}

			BusDriver.Instance.AddBus(ConfigurationCheckerBusName, string.Format("loopback://localhost/{0}", ConfigurationCheckerBusName), x =>
			{
				x.ImplementSagaDistributorWorker(_testConfigurationSagaRepository);
			});

			BusDriver.Instance.AddBus(FileMatcherBusName, string.Format("loopback://localhost/{0}", FileMatcherBusName), x =>
			{
				x.ImplementSagaDistributorWorker(_fileMatcherSagaRepository);
			});
	
			BusDriver.Instance.AddBus(CommandManagerBusName, string.Format("loopback://localhost/{0}", CommandManagerBusName), x =>
			{
				x.SetConcurrentConsumerLimit(4);
				x.UseSagaDistributorFor<TestConfigurationSaga>();
				x.UseSagaDistributorFor<FileMatcherSaga>();
				x.Subscribe((subscriber)=>{

				    subscriber.Consumer<CreateTempDirectoryMessageHandler>();
					subscriber.Consumer<MoveFileToBeProcessedIntoTempDirectoryMessageHandler>();
					subscriber.Consumer<ProcessFileMatchesMessageHandler>();
					subscriber.Consumer<MoveProcessedFileIntoCompletedDirectoryMessageHandler>();
					subscriber.Consumer<DeleteTempDirectoryMessageHandler>();
				});
			});
		}

		public void Stop()
		{
			foreach (var commandService in Container.GetExportedValues<ICommandService>())
			{
				commandService.Stop();
			}

			BusDriver.Instance.RemoveBus(ConfigurationCheckerBusName);
			BusDriver.Instance.RemoveBus(FileMatcherBusName);
			BusDriver.Instance.RemoveBus(CommandManagerBusName);
			_testConfigurationSagaRepository = null;
		}

		private static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>() where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();
			return sagaRepository;
		}

		private ExportProvider Container
		{
			get { return CurrentConfiguration.Container; }
		}
	}
}
