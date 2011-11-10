using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using NLog;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.FileMatcher.Messages;
using Talifun.Commander.Command.FolderWatcher.Messages;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.FileMatcher
{
	[Serializable]
	public class FileMatcherSaga : SagaStateMachine<FileMatcherSaga>, ISaga,
		InitiatedBy<FileFinishedChangingMessage>,
		Orchestrates<CreateTempDirectoryMessage>,
		Orchestrates<MoveFileToBeProcessedIntoTempDirectoryMessage>,
		Orchestrates<ProcessFileMatchesMessage>,
		Orchestrates<MoveProcessedFileIntoCompletedDirectoryMessage>,
		Orchestrates<DeleteTempDirectoryMessage>
	{
		static FileMatcherSaga()
		{
			Define(() =>
			{
				Initially(
					When(Initialised)
						.TransitionTo(WaitingForCreateTempDirectory)
						.Publish((saga, message) => new CreateTempDirectoryMessage()
						{
							CorrelationId = message.CorrelationId
						})	
					);

				During(
					WaitingForCreateTempDirectory,
					When(CreatedTempDirectory)
						.TransitionTo(WaitingForMoveFileToBeProcessedIntoTempDirectory)
						.Publish((saga, message) => new MoveFileToBeProcessedIntoTempDirectoryMessage()
						{
							CorrelationId = message.CorrelationId
						})
				);

				During(
					WaitingForMoveFileToBeProcessedIntoTempDirectory,
					When(MovedFileToBeProcessedIntoTempDirectory)
						.TransitionTo(WaitingForProcessFileMatches)
						.Publish((saga, message) => new ProcessFileMatchesMessage()
						{
							CorrelationId = message.CorrelationId
						})
				);

				During(
					WaitingForProcessFileMatches,
					When(ProcessedFileMatches)
						.TransitionTo(WaitingForMoveProcessedFileIntoCompletedDirectory)
						.Publish((saga, message) => new MoveProcessedFileIntoCompletedDirectoryMessage()
						{
							CorrelationId = message.CorrelationId
						})
						
				);

				During(
					WaitingForMoveProcessedFileIntoCompletedDirectory,
					When(MovedProcessedFileIntoCompletedDirectory)
						.TransitionTo(WaitingForDeleteTempDirectory)
						.Publish((saga, message) => new DeleteTempDirectoryMessage()
						{
							CorrelationId = message.CorrelationId
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

		private string FilePath { get; set; }
		private FolderElement Folder { get; set; }
		private string WorkingFilePath { get; set; }

#region Initialise
		public static Event<FileFinishedChangingMessage> Initialised { get; set; }
		public void Consume(FileFinishedChangingMessage message)
		{
			FilePath = message.FilePath;
			Folder = message.Folder;
			RaiseEvent(Initialised, message);
		}
#endregion

#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }

		public void Consume(CreateTempDirectoryMessage message)
		{
			var fileInfo = new FileInfo(FilePath);
			if (!fileInfo.Exists)
			{
				return;
			}

			fileInfo.WaitForFileToUnlock(10, 500);

			var fileName = fileInfo.Name;
			var uniqueDirectoryName = "master." + fileName + "." + Guid.NewGuid();

			var workingPath = Folder.GetWorkingPathOrDefault();
			var workingDirectoryPath = !string.IsNullOrEmpty(workingPath) ?
				new DirectoryInfo(Path.Combine(workingPath, uniqueDirectoryName))
				: new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));

			workingDirectoryPath.Create();

			WorkingFilePath = Path.Combine(workingDirectoryPath.FullName, fileName);

			var tempDirectoryCreatedMessage = new CreatedTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			RaiseEvent(CreatedTempDirectory, tempDirectoryCreatedMessage);
		}
#endregion

#region Move file to be processed into temp directory
		public static State WaitingForMoveFileToBeProcessedIntoTempDirectory { get; set; }
		public static Event<MovedFileToBeProcessedIntoTempDirectoryMessage> MovedFileToBeProcessedIntoTempDirectory { get; set; }
		public static Event<Fault<MoveFileToBeProcessedIntoTempDirectoryMessage, Guid>> MoveFileToBeProcessedIntoTempDirectoryFailed { get; set; }

		public void Consume(MoveFileToBeProcessedIntoTempDirectoryMessage message)
		{
			var fileInfo = new FileInfo(FilePath);
			fileInfo.WaitForFileToUnlock(10, 500);
			fileInfo.Refresh();
			fileInfo.MoveTo(WorkingFilePath);

			var movedFileToBeProcessedIntoTempDirectoryMessage = new MovedFileToBeProcessedIntoTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			RaiseEvent(MovedFileToBeProcessedIntoTempDirectory, movedFileToBeProcessedIntoTempDirectoryMessage);
		}
#endregion

#region Process file matches
		public static State WaitingForProcessFileMatches { get; set; }
		public static Event<ProcessedFileMatchesMessage> ProcessedFileMatches { get; set; }
		public static Event<Fault<ProcessFileMatchesMessage, Guid>> ProcessFileMatchesFailed { get; set; }

		public void Consume(ProcessFileMatchesMessage message)
		{
			const RegexOptions regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;
			var fileMatchSettings = Folder.FileMatches;

			for (var i = 0; i < fileMatchSettings.Count; i++)
			{
				var workingFilePath = new FileInfo(WorkingFilePath);
				var fileMatch = fileMatchSettings[i];

				var fileNameMatched = true;
				if (!string.IsNullOrEmpty(fileMatch.Expression))
				{
					fileNameMatched = Regex.IsMatch(workingFilePath.Name, fileMatch.Expression, regxOptions);
				}

				if (!fileNameMatched) continue;

				ProcessFileMatch(workingFilePath, fileMatch);

				//If the file no longer exists, it assumed that there should be no more processing
				//e.g. anti-virus may delete file so, we will do no more processing
				//e.g. video process was unable to process file so it was moved to error processing folder
				if (!workingFilePath.Exists || fileMatch.StopProcessing)
				{
					break;
				}

				//Make sure that processing on file has stopped
				workingFilePath.WaitForFileToUnlock(10, 500);
				workingFilePath.Refresh();
			}

			var processedFileMatchesMessage = new ProcessedFileMatchesMessage()
			{
				CorrelationId = message.CorrelationId
			};

			RaiseEvent(ProcessedFileMatches, processedFileMatchesMessage);
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
						if (fileMatches[k] == fileMatch) return projects[i];
					}
				}
			}

			throw new Exception(string.Format(Resource.ErrorMessageCannotFindProjectForFileElement));
		}

		private ICommandSaga GetCommandSaga(string conversionType)
		{
			var commandRunner = Container.GetExportedValues<ICommandSaga>()
				.Where(x => x.Settings.ConversionType == conversionType)
				.First();

			return commandRunner;
		}

		private void ProcessFileMatch(FileInfo fileInfo, FileMatchElement fileMatch)
		{
			var project = GetCurrentProject(fileMatch);
			var commandSaga = GetCommandSaga(fileMatch.ConversionType);
			var commandSagaProperties = new CommandSagaProperties
			{
				FileMatch = fileMatch,
				InputFilePath = fileInfo.FullName,
				Project = project,
				AppSettings = AppSettings
			};

			var logger = LogManager.GetLogger(commandSaga.Settings.ElementType.FullName);
			logger.Info(string.Format(Resource.InfoMessageSagaStarted, project.Name, fileMatch.Name, fileInfo));
			commandSaga.Run(commandSagaProperties);
			logger.Info(string.Format(Resource.InfoMessageSagaCompleted, project.Name, fileMatch.Name, fileInfo));
		}
#endregion

#region Move Processed File To Original Directory
		public static State WaitingForMoveProcessedFileIntoCompletedDirectory { get; set; }
		public static Event<MovedProcessedFileIntoCompletedDirectoryMessage> MovedProcessedFileIntoCompletedDirectory { get; set; }
		public static Event<Fault<MoveProcessedFileIntoCompletedDirectoryMessage, Guid>> MoveProcessedFileIntoCompletedDirectoryMessageFailed { get; set; }

		public void Consume(MoveProcessedFileIntoCompletedDirectoryMessage message)
		{
			var workingFilePath = new FileInfo(WorkingFilePath);
			var completedPath = Folder.GetCompletedPathOrDefault();
			if (!string.IsNullOrEmpty(completedPath) && workingFilePath.Exists)
			{
				var completedFilePath = new FileInfo(Path.Combine(completedPath, workingFilePath.Name));
				if (completedFilePath.Exists)
				{
					completedFilePath.Delete();
				}

				//Make sure that processing on file has stopped
				workingFilePath.WaitForFileToUnlock(10, 500);
				workingFilePath.Refresh();
				workingFilePath.MoveTo(completedFilePath.FullName);
			}

			var movedProcessedFileIntoCompletedDirectoryMessage = new MovedProcessedFileIntoCompletedDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			RaiseEvent(MovedProcessedFileIntoCompletedDirectory, movedProcessedFileIntoCompletedDirectoryMessage);
		}
#endregion

#region Delete Temp Directory
		public static State WaitingForDeleteTempDirectory { get; set; }
		public static Event<DeletedTempDirectoryMessage> DeletedTempDirectory { get; set; }
		public static Event<Fault<DeleteTempDirectoryMessage, Guid>> DeleteTempDirectoryFailed { get; set; }

		public void Consume(DeleteTempDirectoryMessage message)
		{
			var workingDirectoryPath = new FileInfo(WorkingFilePath).Directory;
			if (workingDirectoryPath.Exists)
			{
				workingDirectoryPath.Delete(true);
			}

			var deletedTempDirectoryMessage = new DeletedTempDirectoryMessage()
			{
				CorrelationId = message.CorrelationId
			};

			RaiseEvent(DeletedTempDirectory, deletedTempDirectoryMessage);
		}
#endregion
	}
}
