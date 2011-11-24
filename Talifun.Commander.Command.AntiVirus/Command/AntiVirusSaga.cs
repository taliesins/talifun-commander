using System;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.AntiVirus.Command.Events;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.Command.Response;
using Talifun.Commander.Command.Configuration;

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
						.Publish((saga, message) => new AntiVirusStartedMessage
						{
							CorrelationId = message.CorrelationId,
							WorkingFilePath = message.WorkingFilePath,
							FileMatch = message.FileMatch
						})
						.Then((saga, message) =>
						{
							saga.FileMatch = message.FileMatch;
							saga.WorkingFilePath = message.WorkingFilePath;
							saga.Consume(message);
						})
						.Publish((saga, message) => new AntiVirusCompletedMessage
						{
							CorrelationId = message.CorrelationId,
							WorkingFilePath = message.WorkingFilePath,
							FileMatch = message.FileMatch
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
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string WorkingFilePath { get; set; }

		private void Consume(AntiVirusRequestMessage message)
		{



			var responseMessage = new AntiVirusResponseMessage()
			{
				CorrelationId = message.CorrelationId,
				FileMatch = FileMatch,
				WorkingFilePath = WorkingFilePath
			};
			Bus.Publish(responseMessage);
		}
	}
}
