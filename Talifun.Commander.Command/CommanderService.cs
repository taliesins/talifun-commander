﻿using System.ComponentModel.Composition.Hosting;
using System.Threading;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher;

namespace Talifun.Commander.Command
{
	public class CommanderService
	{
		public const string CommandManagerBusName = "CommandManager";
		private ISagaRepository<ConfigurationCheckerSaga> _testConfigurationSagaRepository;
		private ISagaRepository<FileMatcherSaga> _fileMatcherSagaRepository;

		public void Start()
		{
			_testConfigurationSagaRepository = SetupSagaRepository<ConfigurationCheckerSaga>();
			_fileMatcherSagaRepository = SetupSagaRepository<FileMatcherSaga>();

			foreach (var commandService in Container.GetExportedValues<ICommandService>())
			{
				commandService.Start();
			}

			BusDriver.Instance.AddBus(CommandManagerBusName, string.Format("loopback://localhost/{0}", CommandManagerBusName), x =>
			{
				x.Subscribe((subscriber)=>{
				    subscriber.Saga(_testConfigurationSagaRepository);
					subscriber.Consumer<TestProjectConfigurationMessageHandler>();
					subscriber.Saga(_fileMatcherSagaRepository);
				    subscriber.Consumer<CreateTempDirectoryMessageHandler>();
					subscriber.Consumer<MoveFileToBeProcessedIntoTempDirectoryMessageHandler>();
					subscriber.Consumer<MoveProcessedFileIntoCompletedDirectoryMessageHandler>();
					subscriber.Consumer<DeleteTempDirectoryMessageHandler>();
				});
			});

			//Give the exchange some time to register correctly
			Thread.Yield();
			Thread.Sleep(50);
			Thread.Yield();
			Thread.Sleep(50);
			Thread.Yield();
		}

		public void Stop()
		{
			foreach (var commandService in Container.GetExportedValues<ICommandService>())
			{
				commandService.Stop();
			}

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
