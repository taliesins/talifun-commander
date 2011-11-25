using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Util;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Esb.Response;
using Talifun.Commander.Command.FileMatcher.Request;
using Talifun.Commander.Command.FileMatcher.Response;
using Talifun.Commander.Command.FolderWatcher.Messages;
using Talifun.Commander.Command.Properties;

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
					When(FileFinishedChangingEvent)
						.Then((saga, message)=>
						{
							saga.InputFilePath = message.FilePath;
							saga.Folder = message.Folder;
						})
						.TransitionTo(WaitingForCreateTempDirectory)
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
						    CorrelationId = message.CorrelationId,
							InputFilePath = saga.InputFilePath,
							WorkingPath = saga.Folder.GetWorkingPathOrDefault()
						})
					);

				During(
					WaitingForCreateTempDirectory,
					When(CreatedTempDirectory)
						.Then((saga, message) =>
						{
						    saga.WorkingFilePath = message.WorkingFilePath;
						})
						.TransitionTo(WaitingForMoveFileToBeProcessedIntoTempDirectory)
						.Publish((saga, message) => new MoveFileToBeProcessedIntoTempDirectoryMessage
						{
							CorrelationId = message.CorrelationId,
							FilePath = saga.InputFilePath,
							WorkingFilePath = saga.WorkingFilePath
						})
				);

				During(
					WaitingForMoveFileToBeProcessedIntoTempDirectory,
					When(MovedFileToBeProcessedIntoTempDirectory)
					    .Then((saga, message) =>
					    {
							saga.FileMatchesToExecute = saga.GetFileMatchesToExecute();      		
					    })
						.TransitionTo(WaitingForCommandsToProcess)
						.Publish((saga, message) => new ProcessFileMatchesMessage
						{
							CorrelationId = message.CorrelationId,
							WorkingFilePath = saga.WorkingFilePath,
							FileMatches = saga.FileMatchesToExecute
						})
				);

				During(
					WaitingForCommandsToProcess,
					When(StartCommandExecution)
						.Then((saga, message) =>
						{
							saga.RaiseEvent(ExecuteNextCommand, new ExecuteNextCommandMessage
							{
							    CorrelationId = message.CorrelationId
							});
						}),
					When(ExecuteNextCommand)
						.TransitionTo(WaitingForCommandToExecute)
						.Then((saga, message) =>
						{
							saga.ExecuteCommand(message);    		
						})
					);

				During(
					WaitingForCommandToExecute,
					When(CommandResponse)
						.TransitionTo(WaitingForCommandsToProcess)
						.Then((saga, message) =>
						{
							saga.FileMatchesToExecute.Remove(message.FileMatch.Name);
							saga.RaiseEvent(ExecuteNextCommand, new ExecuteNextCommandMessage
							{
								CorrelationId = message.CorrelationId
							});
						}),
					When(ProcessedFileMatches)
						.TransitionTo(WaitingForMoveProcessedFileIntoCompletedDirectory)
						.Publish((saga, message) => new MoveProcessedFileIntoCompletedDirectoryMessage
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
						.Publish((saga, message) => new DeleteTempDirectoryMessage
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

		[UsedImplicitly]
		public FileMatcherSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual FolderElement Folder { get; set; }
		public virtual string WorkingFilePath { get; set; }
		public virtual FileMatchElementCollection FileMatchesToExecute { get; set; }
		public virtual bool StopProcessingFileMatches { get; set; }

#region Initialise
		public static Event<FileFinishedChangingMessage> FileFinishedChangingEvent { get; set; }
#endregion

#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
#endregion

#region Move file to be processed into temp directory
		public static State WaitingForMoveFileToBeProcessedIntoTempDirectory { get; set; }
		public static Event<MovedFileToBeProcessedIntoTempDirectoryMessage> MovedFileToBeProcessedIntoTempDirectory { get; set; }
		//public static Event<Fault<MoveFileToBeProcessedIntoTempDirectoryMessage, Guid>> MoveFileToBeProcessedIntoTempDirectoryFailed { get; set; }

		private FileMatchElementCollection GetFileMatchesToExecute()
		{
			var fileMatchesToExecute = new FileMatchElementCollection();
			const RegexOptions regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;

			var workingFilePath = new FileInfo(WorkingFilePath);

			foreach (var fileMatch in Folder.FileMatches)
			{
				if (string.IsNullOrEmpty(fileMatch.Expression) || Regex.IsMatch(workingFilePath.Name, fileMatch.Expression, regxOptions))
				{
					fileMatchesToExecute.Add(fileMatch);
				}
			}

			return fileMatchesToExecute;
		}
#endregion

#region Process file matches

		public static State WaitingForCommandsToProcess { get; set; }
		public static Event<ProcessFileMatchesMessage> StartCommandExecution { get; set; }
		public static Event<ExecuteNextCommandMessage> ExecuteNextCommand { get; set; }

		public static State WaitingForCommandToExecute { get; set; }
		public static Event<ICommandResponseMessage> CommandResponse { get; set; }
		//public static Event<Fault<ICommandResponseMessage, Guid>> CommandResponseFailed { get; set; }
		public static Event<ProcessedFileMatchesMessage> ProcessedFileMatches { get; set; }
		
		private void ExecuteCommand(ExecuteNextCommandMessage message)
		{
			var workingFilePath = new FileInfo(WorkingFilePath);

			var fileMatchToExecute = FileMatchesToExecute.FirstOrDefault();

			//If the file no longer exists, it assumed that there should be no more processing
			//e.g. anti-virus may delete file so, we will do no more processing
			//e.g. video process was unable to process file so it was moved to error processing folder
			if (fileMatchToExecute == null || StopProcessingFileMatches || !workingFilePath.Exists)
			{
				//Transition to ProcessedFileMatches
				var processedFileMatchesMessage = new ProcessedFileMatchesMessage
				              	{
									CorrelationId = message.CorrelationId
				              	};

				RaiseEvent(ProcessedFileMatches, processedFileMatchesMessage);
				return;
			}

			if (fileMatchToExecute.StopProcessing)
			{
				StopProcessingFileMatches = true;
			}

			var project = GetCurrentProject(fileMatchToExecute);

			var commandConfigurationTester = GetCommandMessenger(fileMatchToExecute.ConversionType);
			var requestMessage = commandConfigurationTester.CreateRequestMessage(Guid.NewGuid(), AppSettings.Settings.ToDictionary(), project, WorkingFilePath, fileMatchToExecute);

			Bus.Publish(requestMessage.GetType(), requestMessage);
		}

		private ICommandMessenger GetCommandMessenger(string conversionType)
		{
			var commandConfigurationTesters = Container.GetExportedValues<ICommandMessenger>();
			var commandConfigurationTester = commandConfigurationTesters
				.Where(x => x.Settings.ConversionType == conversionType)
				.First();

			return commandConfigurationTester;
		}

		private CommanderSection CommanderSettings
		{
			get { return CurrentConfiguration.CommanderSettings; }
		}

		private AppSettingsSection AppSettings
		{
			get { return CurrentConfiguration.AppSettings; }
		}

		private ExportProvider Container
		{
			get { return CurrentConfiguration.Container; }
		}

		private ProjectElement GetCurrentProject(FileMatchElement fileMatch)
		{
			var projects = CommanderSettings.Projects;

			for (var i = 0; i < projects.Count; i++)
			{
				var folders = projects[i].Folders;

				for (var j = 0; j < folders.Count; j++)
				{
					var fileMatches = folders[j].FileMatches;

					for (var k = 0; k < fileMatches.Count; k++)
					{
						if (fileMatches[k].Name == fileMatch.Name) return projects[i];
					}
				}
			}

			throw new Exception(string.Format(Resource.ErrorMessageCannotFindProjectForFileElement));
		}

#endregion

#region Move Processed File To Original Directory
		public static State WaitingForMoveProcessedFileIntoCompletedDirectory { get; set; }
		public static Event<MovedProcessedFileIntoCompletedDirectoryMessage> MovedProcessedFileIntoCompletedDirectory { get; set; }
		//public static Event<Fault<MoveProcessedFileIntoCompletedDirectoryMessage, Guid>> MoveProcessedFileIntoCompletedDirectoryMessageFailed { get; set; }
#endregion

#region Delete Temp Directory
		public static State WaitingForDeleteTempDirectory { get; set; }
		public static Event<DeletedTempDirectoryMessage> DeletedTempDirectory { get; set; }
		//public static Event<Fault<DeleteTempDirectoryMessage, Guid>> DeleteTempDirectoryFailed { get; set; }
#endregion
	}
}
