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
using Talifun.Commander.Command.FolderWatcher;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Esb
{
	[Serializable]
	public class FileMatchedSaga : SagaStateMachine<FileMatchedSaga>, ISaga
	{
		static FileMatchedSaga()
		{
			Define(() =>
			{
				Initially(
					When(FileFinishedChangingEvent)
						.Then((saga, message) => saga.ProcessFileMatches(message))
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

		public static Event<FileFinishedChangingMessage> FileFinishedChangingEvent { get; set; }


		public FileMatchedSaga(Guid correlationId)
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

		private void ProcessFileMatches(FileFinishedChangingMessage message)
		{
			var folderSetting = message.Folder;
			var fileInfo = new FileInfo(message.FilePath);
			if (!fileInfo.Exists)
			{
				return;
			}

			fileInfo.WaitForFileToUnlock(10, 500);

			var fileName = fileInfo.Name;

			const RegexOptions regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;
			var fileMatchSettings = folderSetting.FileMatches;

			var uniqueDirectoryName = "master." + fileName + "." + Guid.NewGuid();

			var workingPath = folderSetting.GetWorkingPathOrDefault();
			var workingDirectoryPath = !string.IsNullOrEmpty(workingPath) ?
				new DirectoryInfo(Path.Combine(workingPath, uniqueDirectoryName))
				: new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));

			var workingFilePath = new FileInfo(Path.Combine(workingDirectoryPath.FullName, fileName));
			try
			{
				workingDirectoryPath.Create();

				fileInfo.WaitForFileToUnlock(10, 500);
				fileInfo.Refresh();
				fileInfo.MoveTo(workingFilePath.FullName);

				for (var i = 0; i < fileMatchSettings.Count; i++)
				{
					var fileMatch = fileMatchSettings[i];

					var fileNameMatched = true;
					if (!string.IsNullOrEmpty(fileMatch.Expression))
					{
						fileNameMatched = Regex.IsMatch(fileName, fileMatch.Expression, regxOptions);
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
			}
			finally
			{
				var completedPath = folderSetting.GetCompletedPathOrDefault();
				if (!string.IsNullOrEmpty(completedPath) && workingFilePath.Exists)
				{
					var completedFilePath = new FileInfo(Path.Combine(completedPath, fileName));
					if (completedFilePath.Exists)
					{
						completedFilePath.Delete();
					}

					//Make sure that processing on file has stopped
					workingFilePath.WaitForFileToUnlock(10, 500);
					workingFilePath.Refresh();
					workingFilePath.MoveTo(completedFilePath.FullName);
				}

				if (workingDirectoryPath.Exists)
				{
					workingDirectoryPath.Delete(true);
				}
			}
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
				InputFilePath = fileInfo,
				Project = project,
				AppSettings = AppSettings
			};

			var logger = LogManager.GetLogger(commandSaga.Settings.ElementType.FullName);
			logger.Info(string.Format(Resource.InfoMessageSagaStarted, project.Name, fileMatch.Name, fileInfo));
			commandSaga.Run(commandSagaProperties);
			logger.Info(string.Format(Resource.InfoMessageSagaCompleted, project.Name, fileMatch.Name, fileInfo));
		}
	}
}
