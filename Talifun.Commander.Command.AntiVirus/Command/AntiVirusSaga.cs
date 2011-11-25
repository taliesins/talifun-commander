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
							InputFilePath = message.InputFilePath,
							FileMatch = message.FileMatch
						})
						.Then((saga, message) =>
						{
							saga.FileMatch = message.FileMatch;
							saga.InputFilePath = message.InputFilePath;
							saga.Consume(message);
						})
						.TransitionTo(WaitingForCreateTempDirectory)
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = message.CorrelationId,
							InputFilePath = saga.InputFilePath
						})
					);

				//During(
				//    WaitingForCreateTempDirectory,
				//    When(CreatedTempDirectory)
				//        .Then((saga, message) =>
				//        {
				//            saga.WorkingPath = message.WorkingPath;
				//        })
				//        .TransitionTo(WaitingForMoveFileToBeProcessedIntoTempDirectory)
				//        .Publish((saga, message) => new MoveFileToBeProcessedIntoTempDirectoryMessage
				//        {
				//            CorrelationId = message.CorrelationId,
				//            FilePath = saga.FilePath,
				//            WorkingFilePath = saga.InputFilePath
				//        })
				//);



				During(
					WaitingForMoveProcessedFileIntoOutputDirectory,
					When(MovedProcessedFileIntoOutputDirectory)
						.TransitionTo(WaitingForDeleteTempDirectory)
						.Publish((saga, message) => new DeleteTempDirectoryMessage
						{
							CorrelationId = message.CorrelationId,
							WorkingPath = saga.WorkingPath
						})
				);

				During(
					WaitingForDeleteTempDirectory,
					When(DeletedTempDirectory)
						.Publish((saga, message) => new AntiVirusCompletedMessage
						{
							CorrelationId = message.CorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
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

		public AntiVirusSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual string WorkingPath { get; set; }

		#region Initialise
		public static Event<AntiVirusRequestMessage> AntiVirusRequestEvent { get; set; }
		#endregion

		#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
		#endregion

		#region Move Processed File To Original Directory
		public static State WaitingForMoveProcessedFileIntoOutputDirectory { get; set; }
		public static Event<MovedProcessedFileIntoOutputDirectoryMessage> MovedProcessedFileIntoOutputDirectory { get; set; }
		//public static Event<Fault<MoveProcessedFileIntoCompletedDirectoryMessage, Guid>> MoveProcessedFileIntoOutputDirectoryMessageFailed { get; set; }
		#endregion

		#region Delete Temp Directory
		public static State WaitingForDeleteTempDirectory { get; set; }
		public static Event<DeletedTempDirectoryMessage> DeletedTempDirectory { get; set; }
		//public static Event<Fault<DeleteTempDirectoryMessage, Guid>> DeleteTempDirectoryFailed { get; set; }
		#endregion


		private void Consume(AntiVirusRequestMessage message)
		{



			var responseMessage = new AntiVirusResponseMessage()
			{
				CorrelationId = message.CorrelationId,
				FileMatch = FileMatch,
				InputFilePath = InputFilePath
			};
			Bus.Publish(responseMessage);
		}
	}
}
