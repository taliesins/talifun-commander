using MassTransit.Distributor;
using MassTransit.Saga;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command
{
	public abstract class CommandServiceBase<T> : ICommandService where T : SagaStateMachine<T>, ISaga
	{
		private ISagaRepository<T> _commandSagaRepository;

		public abstract ISettingConfiguration Settings { get; }

		public virtual string BusName
		{
			get { return Settings.ElementSettingName; }
		}

		public virtual string SagaBusName
		{
			get { return string.Format("{0}_saga", Settings.ElementSettingName); }
		}

		public void Start()
		{
			_commandSagaRepository = SetupSagaRepository<T>();

			BusDriver.Instance.AddBus(SagaBusName, string.Format("loopback://localhost/{0}", SagaBusName), x =>
			{
				x.ImplementSagaDistributorWorker(_commandSagaRepository);
			});

			BusDriver.Instance.AddBus(BusName, string.Format("loopback://localhost/{0}", BusName), x =>
			{
				x.UseSagaDistributorFor<T>();
			});
		}

		public void Stop()
		{
			BusDriver.Instance.RemoveBus(SagaBusName);
			BusDriver.Instance.RemoveBus(BusName);
		}

		protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>() where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();
			return sagaRepository;
		}
	}
}
