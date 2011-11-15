using System;
using System.Linq;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.FileMatcher.Messages;
using Talifun.Commander.Command.FolderWatcher.Messages;

namespace Talifun.Commander.Command.FileMatcher
{
	[Serializable]
	public class FileMatcherSaga : SagaStateMachine<FileMatcherSaga>, ISaga
	{
		static FileMatcherSaga()
		{
			Define(() =>
			{
				Initially(
					When(Initialised)
						.Then((saga, message) =>
						      	{
									saga.FilePath = message.FilePath;
									saga.Folder = message.Folder;
						      		var createTempDirectoryMessage = new CreateTempDirectoryMessage()
						      			{
						      				CorrelationId = message.CorrelationId,
											FilePath = saga.FilePath,
											WorkingPath = saga.Folder.GetWorkingPathOrDefault()
						      			};
									saga.Bus.Publish(createTempDirectoryMessage);
						      	}).Where(x => x.Folder.FileMatches.Any())

						.TransitionTo(WaitingForCreateTempDirectory).Where(x => x.Folder.FileMatches.Any())
					);

				During(
					WaitingForCreateTempDirectory,
					When(CreatedTempDirectory)
						.Then((saga, message) =>
						{
						    saga.WorkingFilePath = message.WorkingFilePath;
						})
						.TransitionTo(WaitingForMoveFileToBeProcessedIntoTempDirectory)
						.Publish((saga, message) => new MoveFileToBeProcessedIntoTempDirectoryMessage()
						{
							CorrelationId = message.CorrelationId,
							FilePath = saga.FilePath,
							WorkingFilePath = saga.WorkingFilePath
						})
				);

				During(
					WaitingForMoveFileToBeProcessedIntoTempDirectory,
					When(MovedFileToBeProcessedIntoTempDirectory)
						.TransitionTo(WaitingForProcessFileMatches)
						.Publish((saga, message) => new ProcessFileMatchesMessage()
						{
							CorrelationId = message.CorrelationId,
							WorkingFilePath = saga.WorkingFilePath,
							FileMatches = saga.Folder.FileMatches
						})
				);

				During(
					WaitingForProcessFileMatches,
					When(ProcessedFileMatches)
						.TransitionTo(WaitingForMoveProcessedFileIntoCompletedDirectory)
						.Publish((saga, message) => new MoveProcessedFileIntoCompletedDirectoryMessage()
						{
							CorrelationId = message.CorrelationId,
							WorkingFilePath = saga.WorkingFilePath,
							CompletedPath = saga.Folder.GetCompletedPathOrDefault()
						})
				);

				During(
					WaitingForMoveProcessedFileIntoCompletedDirectory,
					When(MovedProcessedFileIntoCompletedDirectory)
						.TransitionTo(WaitingForDeleteTempDirectory)
						.Publish((saga, message) => new DeleteTempDirectoryMessage()
						{
							CorrelationId = message.CorrelationId,
							WorkingFilePath = saga.WorkingFilePath
						})
				);

				During(
					WaitingForDeleteTempDirectory,
					When(DeletedTempDirectory)
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

		public FileMatcherSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public Guid CorrelationId { get; private set; }
		public IServiceBus Bus { get; set; }
		
		private string FilePath { get; set; }
		private FolderElement Folder { get; set; }
		private string WorkingFilePath { get; set; }

#region Initialise
		public static Event<FileFinishedChangingMessage> Initialised { get; set; }
#endregion

#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
#endregion

#region Move file to be processed into temp directory
		public static State WaitingForMoveFileToBeProcessedIntoTempDirectory { get; set; }
		public static Event<MovedFileToBeProcessedIntoTempDirectoryMessage> MovedFileToBeProcessedIntoTempDirectory { get; set; }
		public static Event<Fault<MoveFileToBeProcessedIntoTempDirectoryMessage, Guid>> MoveFileToBeProcessedIntoTempDirectoryFailed { get; set; }
#endregion

#region Process file matches
		public static State WaitingForProcessFileMatches { get; set; }
		public static Event<ProcessedFileMatchesMessage> ProcessedFileMatches { get; set; }
		public static Event<Fault<ProcessFileMatchesMessage, Guid>> ProcessFileMatchesFailed { get; set; }
#endregion

#region Move Processed File To Original Directory
		public static State WaitingForMoveProcessedFileIntoCompletedDirectory { get; set; }
		public static Event<MovedProcessedFileIntoCompletedDirectoryMessage> MovedProcessedFileIntoCompletedDirectory { get; set; }
		public static Event<Fault<MoveProcessedFileIntoCompletedDirectoryMessage, Guid>> MoveProcessedFileIntoCompletedDirectoryMessageFailed { get; set; }
#endregion

#region Delete Temp Directory
		public static State WaitingForDeleteTempDirectory { get; set; }
		public static Event<DeletedTempDirectoryMessage> DeletedTempDirectory { get; set; }
		public static Event<Fault<DeleteTempDirectoryMessage, Guid>> DeleteTempDirectoryFailed { get; set; }
#endregion
	}
}
