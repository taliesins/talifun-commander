using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.CommandLine.Properties;
using Talifun.Commander.Command.Configuration;

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

                if (commandLineSetting.CheckCommandPathExists && !File.Exists(commandLineSetting.CommandPath))
                {
                	throw new Exception(
                		string.Format(Resource.ErrorMessageCommandPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              commandLineSetting.Name,
                		              commandLineSetting.CommandPath));
                }

                if (!Directory.Exists(commandLineSetting.OutPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              commandLineSetting.Name,
                		              commandLineSetting.OutPutPath));
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(commandLineSetting.OutPutPath));
                }

                if (!string.IsNullOrEmpty(commandLineSetting.WorkingPath))
                {
                    if (!Directory.Exists(commandLineSetting.WorkingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              commandLineSetting.Name,
                    		              commandLineSetting.WorkingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(commandLineSetting.WorkingPath));
                    }
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                }

                if (!string.IsNullOrEmpty(commandLineSetting.ErrorProcessingPath))
                {
                    if (!Directory.Exists(commandLineSetting.ErrorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              commandLineSetting.Name,
                    		              commandLineSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(commandLineSetting.ErrorProcessingPath));
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
