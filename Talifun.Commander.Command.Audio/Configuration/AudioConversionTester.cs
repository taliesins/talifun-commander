using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;

namespace Talifun.Commander.Command.Audio.Configuration
{
    public class AudioConversionTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return AudioConversionConfiguration.Instance;
            }
        }

		public override void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<AudioConversionElementCollection>(Settings.ElementCollectionSettingName, project);
            var audioConversionSettings = commandSettings.Settings;

            var audioConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (audioConversionSettingsKeys.Count <= 0)
            {
                return;
            }

            var ffMpegPath = appSettings.Settings[AudioConversionConfiguration.Instance.FFMpegPathSettingName].Value;

            if (string.IsNullOrEmpty(ffMpegPath))
            {
                throw new Exception(string.Format(Command.Properties.Resource.ErrorMessageAppSettingRequired, AudioConversionConfiguration.Instance.FFMpegPathSettingName));
            }

            
            for (var i = 0; i < audioConversionSettings.Count; i++)
            {
                var audioConversionSetting = audioConversionSettings[i];

            	var outPutPath = audioConversionSetting.GetOutPutPathOrDefault();

				if (!Directory.Exists(outPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              audioConversionSetting.Name,
									  outPutPath));
                }
                else
                {
					TryCreateTestFile(new DirectoryInfo(outPutPath));
                }

            	var workingPath = audioConversionSetting.GetWorkingPathOrDefault();
				if (!string.IsNullOrEmpty(workingPath))
                {
					if (!Directory.Exists(workingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              audioConversionSetting.Name,
										  workingPath));
                    }
                    else
                    {
						TryCreateTestFile(new DirectoryInfo(workingPath));
                    }
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                }

            	var errorProcessingPath = audioConversionSetting.GetErrorProcessingPathOrDefault();
				if (!string.IsNullOrEmpty(errorProcessingPath))
                {
					if (!Directory.Exists(errorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              audioConversionSetting.Name,
										  errorProcessingPath));
                    }
                    else
                    {
						TryCreateTestFile(new DirectoryInfo(errorProcessingPath));
                    }
                }

                audioConversionSettingsKeys.Remove(audioConversionSetting.Name);
            }

            if (audioConversionSettingsKeys.Count > 0)
            {
                FileMatchElement fileMatch = null;
                foreach (var value in audioConversionSettingsKeys.Values)
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
