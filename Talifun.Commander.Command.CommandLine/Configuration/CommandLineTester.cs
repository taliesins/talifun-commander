using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.CommandLine.Properties;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
    public class CommandLineTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return CommandLineConfiguration.Instance;
            }
        }

		public override void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<CommandLineElementCollection>(Settings.ElementCollectionSettingName, project);
            var commandLineSettings = commandSettings.Settings;

            var commandLineSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (commandLineSettingsKeys.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < commandLineSettings.Count; i++)
            {
                var commandLineSetting = commandLineSettings[i];

            	var commandPath = commandLineSetting.CommandPath;
				if (commandLineSetting.CheckCommandPathExists && !File.Exists(commandPath))
                {
                	throw new Exception(
                		string.Format(Resource.ErrorMessageCommandPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              commandLineSetting.Name,
									  commandPath));
                }

				var outPutPath = commandLineSetting.GetOutPutPathOrDefault();
				if (!Directory.Exists(outPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              commandLineSetting.Name,
									  outPutPath));
                }
                else
                {
					(new DirectoryInfo(outPutPath)).TryCreateTestFile();
                }

				var workingPath = commandLineSetting.GetWorkingPathOrDefault();
				if (!string.IsNullOrEmpty(workingPath))
                {
					if (!Directory.Exists(workingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              commandLineSetting.Name,
										  workingPath));
                    }
                    else
                    {
						(new DirectoryInfo(workingPath)).TryCreateTestFile();
                    }
                }
                else
                {
					(new DirectoryInfo(Path.GetTempPath())).TryCreateTestFile();
                }

            	var errorProcessingPath = commandLineSetting.GetErrorProcessingPathOrDefault();
				if (!string.IsNullOrEmpty(errorProcessingPath))
                {
					if (!Directory.Exists(errorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              commandLineSetting.Name,
										  errorProcessingPath));
                    }
                    else
                    {
						(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
                    }
                }

                commandLineSettingsKeys.Remove(commandLineSetting.Name);
            }

            if (commandLineSettingsKeys.Count > 0)
            {
                FileMatchElement fileMatch = null;
                foreach (var value in commandLineSettingsKeys.Values)
                {
                    fileMatch = value;
                    break;
                }

            	throw new Exception(
            		string.Format(
            			Command.Properties.Resource.ErrorMessageCommandConversionSettingKeyPointsToNonExistantCommand,
            			project.Name, fileMatch.Name, Settings.ElementSettingName, fileMatch.CommandSettingsKey));
            }
        }
    }
}
