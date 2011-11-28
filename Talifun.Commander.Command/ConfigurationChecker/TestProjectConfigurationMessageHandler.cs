using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Magnum;
using MassTransit;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker.Request;
using Talifun.Commander.Command.ConfigurationChecker.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.ConfigurationChecker
{
	public class TestProjectConfigurationMessageHandler : Consumes<TestProjectConfigurationMessage>.All
	{
		private ExportProvider Container
		{
			get { return CurrentConfiguration.Container; }
		}

		private ICommandMessenger GetCommandMessenger(string conversionType)
		{
			var commandConfigurationTesters = Container.GetExportedValues<ICommandMessenger>();
			var commandConfigurationTester = commandConfigurationTesters
				.Where(x => x.Settings.ConversionType == conversionType)
				.First();

			return commandConfigurationTester;
		}

		public void Consume(TestProjectConfigurationMessage message)
		{
			var project = message.Project;
			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			var exceptions = new List<Exception>();

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

				try
				{
					//Check that there are no duplicate folderToWatch
					if (foldersToWatch.Contains(folderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchIsADuplicate, project.Name, folderSetting.Name, folderToWatch));
					foldersToWatch.Add(folderToWatch);

					//Check that folder to watch exists
					if (!Directory.Exists(folderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchDoesNotExist, project.Name, folderSetting.Name, folderToWatch));

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
						var testConfigurationRequestMessage = commandConfigurationTester.CreateTestConfigurationRequestMessage(CombGuid.Generate(), message.CorrelationId, message.AppSettings, message.Project);
						
						bus.PublishRequest(testConfigurationRequestMessage.GetType(), testConfigurationRequestMessage, x =>
						{
							x.Handle<IConfigurationTestResponseMessage>(response =>
							{
								exceptions.AddRange(response.Exceptions);
							});
						});
					}
				}
				catch (Exception exception)
				{
					exceptions.Add(exception);
				}
			}

			var testedProjectConfigurationMessage = new TestedProjectConfigurationMessage
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = exceptions
			};

			bus.Publish(testedProjectConfigurationMessage);
		}
	}
}
