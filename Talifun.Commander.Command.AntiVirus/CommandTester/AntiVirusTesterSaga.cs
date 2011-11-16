using System;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;

namespace Talifun.Commander.Command.AntiVirus.CommandTester
{
	[Serializable]
	public class AntiVirusTesterSaga : SagaStateMachine<AntiVirusTesterSaga>, ISaga
	{
		static AntiVirusTesterSaga()
		{
			Define(() =>
			{
			});
		}

		// ReSharper disable UnusedMember.Global
		public static State Initial { get; set; }
		// ReSharper restore UnusedMember.Global
		// ReSharper disable UnusedMember.Global
		public static State Completed { get; set; }
		// ReSharper restore UnusedMember.Global

		public AntiVirusTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public Guid CorrelationId { get; private set; }
		public IServiceBus Bus { get; set; }
	}
}
