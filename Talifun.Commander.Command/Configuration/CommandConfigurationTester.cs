﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

		public override void CheckProjectConfiguration(Configuration.ProjectElement project, NameValueCollection appSettings)
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

                //Check that folder to watch exists
                if (!Directory.Exists(folderSetting.FolderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchDoesNotExist, project.Name, folderSetting.Name, folderSetting.FolderToWatch));

                //Check that there are no duplicate folderToWatch
                if (foldersToWatch.Contains(folderSetting.FolderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchIsADuplicate, project.Name, folderSetting.Name, folderSetting.FolderToWatch));
                foldersToWatch.Add(folderSetting.FolderToWatch);

                //Check that working path is valid
                if (!string.IsNullOrEmpty(folderSetting.WorkingPath))
                {
                    if (!Directory.Exists(folderSetting.WorkingPath)) throw new Exception(string.Format(Resource.ErrorMessageWorkingPathDoesNotExist, project.Name, folderSetting.Name, folderSetting.WorkingPath));
                    else TryCreateTestFile(new DirectoryInfo(folderSetting.WorkingPath));
                }
                else TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));

                //Check completed path is valid
                if (!string.IsNullOrEmpty(folderSetting.CompletedPath))
                {
                    if (!Directory.Exists(folderSetting.CompletedPath)) throw new Exception(string.Format(Resource.ErrorMessageCompletedPathDoesNotExist, project.Name, folderSetting.Name, folderSetting.CompletedPath));
                    else TryCreateTestFile(new DirectoryInfo(folderSetting.CompletedPath));
                }

                var fileMatches = folderSetting.FileMatches;

                for (var j = 0; j < fileMatches.Count; j++)
                {
                    var fileMatch = fileMatches[j];

                    var commandConfigurationTester = GetCommandConfigurationTester(fileMatch.ConversionType);
					commandConfigurationTester.CheckProjectConfiguration(project, appSettings);
                }
            }
        }

        public override ISettingConfiguration Settings
        {
            get { throw new NotImplementedException(); }
        }
    }
}
