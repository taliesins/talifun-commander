using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    [PartNotDiscoverable]
    public class CommandConfigurationTester : CommandConfigurationTesterBase
    {
        protected ExportProvider Container;
        public CommandConfigurationTester(ExportProvider container)
        {
            Container = container;
        }

        public ICommandConfigurationTester GetCommandConfigurationTester(string conversionType)
        {
            var commandConfigurationTesters = Container.GetExportedValues<ICommandConfigurationTester>();
            var commandConfigurationTester = commandConfigurationTesters
                .Where(x => x.Settings.ConversionType == conversionType)
                .First();

            return commandConfigurationTester;
        }

		public override void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
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
					else TryCreateTestFile(new DirectoryInfo(workingPath));
                }
                else TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));

            	var completedPath = folderSetting.GetCompletedPathOrDefault();
                //Check completed path is valid
				if (!string.IsNullOrEmpty(completedPath))
                {
					if (!Directory.Exists(completedPath)) throw new Exception(string.Format(Resource.ErrorMessageCompletedPathDoesNotExist, project.Name, folderSetting.Name, completedPath));
					else TryCreateTestFile(new DirectoryInfo(completedPath));
                }

                var fileMatches = folderSetting.FileMatches;

                for (var j = 0; j < fileMatches.Count; j++)
                {
                    var fileMatch = fileMatches[j];

                    var commandConfigurationTester = GetCommandConfigurationTester(fileMatch.ConversionType);
					commandConfigurationTester.CheckProjectConfiguration(appSettings, project);
                }
            }
        }

        public override ISettingConfiguration Settings
        {
            get { throw new NotImplementedException(); }
        }
    }
}
