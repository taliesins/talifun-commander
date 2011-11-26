using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker.Request;
using Talifun.Commander.Command.ConfigurationChecker.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Esb.Response;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.ConfigurationChecker
{
	[Serializable]
	public class ConfigurationCheckerSaga : SagaStateMachine<ConfigurationCheckerSaga>, ISaga
	{
		static ConfigurationCheckerSaga()
		{
			Define(() =>
			{
				Initially(
					When(TestConfigurationEvent)
						.Then((saga, message) => saga.CheckConfiguration(message))
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

		public static Event<RequestTestConfigurationMessage> TestConfigurationEvent { get; set; }

		public ConfigurationCheckerSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		#region Test Configuration

		private ExportProvider Container
		{
			get { return CurrentConfiguration.Container; }
		}

		private CommanderSection CommanderSettings
		{
			get { return CurrentConfiguration.CommanderSettings; }
		}

		private AppSettingsSection AppSettings
		{
			get { return CurrentConfiguration.AppSettings; }
		}

		private void CheckConfiguration(RequestTestConfigurationMessage message)
		{
			var projects = CommanderSettings.Projects;
			for (var j = 0; j < projects.Count; j++)
			{
				//We need to get all commands

				CheckProjectConfiguration(AppSettings, projects[j]);
			}

			var responseTestConfigurationMessage = new ResponseTestConfigurationMessage
			                                       	{
			                                       		CorrelationId = message.CorrelationId
			                                       	};

			Bus.Publish(responseTestConfigurationMessage);
		}

		private ICommandMessenger GetCommandMessenger(string conversionType)
		{
			var commandConfigurationTesters = Container.GetExportedValues<ICommandMessenger>();
			var commandConfigurationTester = commandConfigurationTesters
				.Where(x => x.Settings.ConversionType == conversionType)
				.First();

			return commandConfigurationTester;
		}

		private void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
		{
			//We only want to check the sections if they are used, otherwise it will complain about
			//sections missing even if we aren't using them.

			var foldersToWatch = new List<string>();

			//This will check that all the required folders exists
			//It will also check that the service has the correct permissions to create, edit and delete files
			var folderSettings = project.Folders;
			for (var i = 0; i < folderSettings.Count; i++)
			{
				var folderSetting = folderSettings[i];

				var folderToWatch = folderSetting.GetFolderToWatchOrDefault();
				//Check that folder to watch exists
				if (!Directory.Exists(folderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchDoesNotExist, project.Name, folderSetting.Name, folderToWatch));

				//Check that there are no duplicate folderToWatch
				if (foldersToWatch.Contains(folderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchIsADuplicate, project.Name, folderSetting.Name, folderToWatch));
				foldersToWatch.Add(folderToWatch);

				var workingPath = folderSetting.GetWorkingPathOrDefault();
				//Check that working path is valid
				if (!string.IsNullOrEmpty(workingPath))
				{
					if (!Directory.Exists(workingPath)) throw new Exception(string.Format(Resource.ErrorMessageWorkingPathDoesNotExist, project.Name, folderSetting.Name, workingPath));
					else (new DirectoryInfo(workingPath)).TryCreateTestFile();
				}
				else (new DirectoryInfo(Path.GetTempPath())).TryCreateTestFile();

				var completedPath = folderSetting.GetCompletedPathOrDefault();
				//Check completed path is valid
				if (!string.IsNullOrEmpty(completedPath))
				{
					if (!Directory.Exists(completedPath)) throw new Exception(string.Format(Resource.ErrorMessageCompletedPathDoesNotExist, project.Name, folderSetting.Name, completedPath));
					else (new DirectoryInfo(completedPath)).TryCreateTestFile();
				}

				var fileMatches = folderSetting.FileMatches;

				for (var j = 0; j < fileMatches.Count; j++)
				{
					var fileMatch = fileMatches[j];

					var commandConfigurationTester = GetCommandMessenger(fileMatch.ConversionType);
					var testConfigurationRequestMessage = commandConfigurationTester.CreateTestConfigurationRequestMessage(Guid.NewGuid(), CorrelationId, appSettings.Settings.ToDictionary(), project);
					
					Bus.PublishRequest(testConfigurationRequestMessage.GetType(), testConfigurationRequestMessage, x =>
					{
						x.Handle<IConfigurationTestResponseMessage>(response =>
						{
							if (response.Exception != null)
							{
								throw response.Exception;
							}
						});
					});
				}
			}
		}

		#endregion
	}
}
