using MassTransit.Distributor;
using MassTransit.Saga;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command
{
	public abstract class CommandServiceBase<TCommandSaga, TCommandTesterSaga> : ICommandService 
		where TCommandSaga : SagaStateMachine<TCommandSaga>, ISaga
		where TCommandTesterSaga : SagaStateMachine<TCommandTesterSaga>, ISaga
	{
		private ISagaRepository<TCommandSaga> _commandSagaRepository;
		private ISagaRepository<TCommandTesterSaga> _commandTesterSagaRepository;

		public abstract ISettingConfiguration Settings { get; }

		public virtual string BusName
		{
			get { return Settings.ElementSettingName; }
		}

		public virtual string CommandSagaBusName
		{
			get { return string.Format("{0}_command", Settings.ElementSettingName); }
		}

		public virtual string CommandTesterSagaBusName
		{
			get { return string.Format("{0}_command_tester", Settings.ElementSettingName); }
		}

		public void Start()
		{
			_commandSagaRepository = SetupSagaRepository<TCommandSaga>();
			_commandTesterSagaRepository = SetupSagaRepository<TCommandTesterSaga>();

			BusDriver.Instance.AddBus(CommandSagaBusName, string.Format("loopback://localhost/{0}", CommandSagaBusName), x =>
			{
				x.ImplementSagaDistributorWorker(_commandSagaRepository);
			});

			BusDriver.Instance.AddBus(CommandTesterSagaBusName, string.Format("loopback://localhost/{0}", CommandTesterSagaBusName), x =>
			{
				x.ImplementSagaDistributorWorker(_commandTesterSagaRepository);
			});

			BusDriver.Instance.AddBus(BusName, string.Format("loopback://localhost/{0}", BusName), x =>
			{
				x.UseSagaDistributorFor<TCommandSaga>();
				x.UseSagaDistributorFor<TCommandTesterSaga>();
			});
		}

		public void Stop()
		{
			BusDriver.Instance.RemoveBus(CommandSagaBusName);
			BusDriver.Instance.RemoveBus(CommandTesterSagaBusName);
			BusDriver.Instance.RemoveBus(BusName);
		}

		protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>() where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();
			return sagaRepository;
		}
	}
}
