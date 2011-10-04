using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Talifun.Commander.Command.Configuration;

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

        public override void CheckProjectConfiguration(ProjectElement project, NameValueCollection appSettings)
        {
            var commandSettings = new ProjectElementCommand<AudioConversionElementCollection>(Settings.ElementCollectionSettingName, project);
            var audioConversionSettings = commandSettings.Settings;

            var audioConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (audioConversionSettingsKeys.Count <= 0)
            {
                return;
            }

            var ffMpegPath = appSettings[AudioConversionConfiguration.Instance.FFMpegPathSettingName];

            if (string.IsNullOrEmpty(ffMpegPath))
            {
                throw new Exception(string.Format(Command.Properties.Resource.ErrorMessageAppSettingRequired, AudioConversionConfiguration.Instance.FFMpegPathSettingName));
            }

            
            for (var i = 0; i < audioConversionSettings.Count; i++)
            {
                var audioSetting = audioConversionSettings[i];

                if (!Directory.Exists(audioSetting.OutPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              audioSetting.Name,
                		              audioSetting.OutPutPath));
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(audioSetting.OutPutPath));
                }

                if (!string.IsNullOrEmpty(audioSetting.WorkingPath))
                {
                    if (!Directory.Exists(audioSetting.WorkingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              audioSetting.Name,
                    		              audioSetting.WorkingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(audioSetting.WorkingPath));
                    }
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                }

                if (!string.IsNullOrEmpty(audioSetting.ErrorProcessingPath))
                {
                    if (!Directory.Exists(audioSetting.ErrorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              audioSetting.Name,
                    		              audioSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(audioSetting.ErrorProcessingPath));
                    }
                }

                audioConversionSettingsKeys.Remove(audioSetting.Name);
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
