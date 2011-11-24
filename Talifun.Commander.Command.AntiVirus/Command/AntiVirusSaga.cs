using System;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.Command.Response;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	[Serializable]
	public class AntiVirusSaga : SagaStateMachine<AntiVirusSaga>, ISaga
	{
		static AntiVirusSaga()
		{
			Define(() =>
			{
				Initially(
					When(AntiVirusRequestEvent)
						.Then((saga, message) =>
						{
							saga.Consume(message);
						})
						.Complete()
					);
			});
		}

		// ReSharper disable UnusedMember.Global
		public static State Initial { get; set; }
		// ReSharper restore UnusedMember.Global
		// ReSharper disable UnusedMember.Global
		public static State Completed { get; set; }
		// ReSharper restore UnusedMember.Global

		public static Event<AntiVirusRequestMessage> AntiVirusRequestEvent { get; set; }

		public AntiVirusSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private void Consume(AntiVirusRequestMessage message)
		{
			var responseMessage = new AntiVirusResponseMessage()
			{
				CorrelationId = message.CorrelationId,
				FileMatch = message.FileMatch,
				WorkingFilePath = message.WorkingFilePath
			};

			Bus.Publish(responseMessage);
		}
	}
}
