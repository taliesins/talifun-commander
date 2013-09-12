using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Saga;

namespace Talifun.Commander.Command
{
	public abstract class CommandServiceBase<TCommandSaga, TCommandTesterSaga> : ICommandService 
		where TCommandSaga : SagaStateMachine<TCommandSaga>, ISaga
		where TCommandTesterSaga : SagaStateMachine<TCommandTesterSaga>, ISaga
	{
		private ISagaRepository<TCommandSaga> _commandSagaRepository;
		private ISagaRepository<TCommandTesterSaga> _commandTesterSagaRepository;

		public static ISettingConfiguration Settings { get; protected set; }

		public static string BusName
		{
			get { return CommanderService.CommandManagerBusName; }
		}

		public void Start()
		{
			_commandSagaRepository = SetupSagaRepository<TCommandSaga>();
			_commandTesterSagaRepository = SetupSagaRepository<TCommandTesterSaga>();
			OnStart();
		}

		public void Configure(ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Saga(_commandSagaRepository).Permanent();
				subscriber.Saga(_commandTesterSagaRepository).Permanent();
			});
			OnConfigure(serviceBusConfigurator);
		}

		public void Stop()
		{
			OnStop();
		}

		public virtual void OnStart()
		{
			
		}

		public virtual void OnConfigure(ServiceBusConfigurator serviceBusConfigurator)
		{
			
		}

		public virtual void OnStop()
		{
			
		}

		protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>() where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();
			return sagaRepository;
		}
	}
}
