using MassTransit;
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

		public static ISettingConfiguration Settings { get; protected set; }

		public static string BusName
		{
			get { return Settings.ElementSettingName; }
		}

		public void Start()
		{
			_commandSagaRepository = SetupSagaRepository<TCommandSaga>();
			_commandTesterSagaRepository = SetupSagaRepository<TCommandTesterSaga>();

			BusDriver.Instance.AddBus(BusName, string.Format("loopback://localhost/{0}", BusName), x =>
			{
				x.Subscribe((subscriber)=> {
					subscriber.Saga(_commandSagaRepository);
					subscriber.Saga(_commandTesterSagaRepository);
				});
			});
		}

		public void Stop()
		{
			BusDriver.Instance.RemoveBus(BusName);
		}

		protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>() where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();
			return sagaRepository;
		}
	}
}
