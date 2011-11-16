using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MassTransit;
using NLog;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher.Request;
using Talifun.Commander.Command.FileMatcher.Response;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.FileMatcher
{
	public class ProcessFileMatchesMessageHandler : Consumes<ProcessFileMatchesMessage>.All
	{
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

		public void Consume(ProcessFileMatchesMessage message)
		{
			const RegexOptions regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;
			var fileMatchSettings = message.FileMatches;

			for (var i = 0; i < fileMatchSettings.Count; i++)
			{
				var workingFilePath = new FileInfo(message.WorkingFilePath);
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

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.Publish(processedFileMatchesMessage, x => { });
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
	}
}
