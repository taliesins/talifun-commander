using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
    public class VideoThumbnailerTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return VideoThumbnailerConfiguration.Instance;
            }
        }

		public override void CheckProjectConfiguration(ProjectElement project, AppSettingsSection appSettings)
        {
            var commandSettings = new ProjectElementCommand<VideoThumbnailerElementCollection>(Settings.ElementCollectionSettingName, project);
            var videoThumbnailerSettings = commandSettings.Settings;

            var videoThumbnailerSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (videoThumbnailerSettingsKeys.Count <= 0)
            {
                return;
            }

			var ffMpegPath = appSettings.Settings[VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName].Value;

            if (string.IsNullOrEmpty(ffMpegPath))
            {
				throw new Exception(string.Format(Command.Properties.Resource.ErrorMessageAppSettingRequired, VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName));
            }

            for (var i = 0; i < videoThumbnailerSettings.Count; i++)
            {
                var videoThumbnailerSetting = videoThumbnailerSettings[i];

                if (!Directory.Exists(videoThumbnailerSetting.OutPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              videoThumbnailerSetting.Name,
                		              videoThumbnailerSetting.OutPutPath));
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(videoThumbnailerSetting.OutPutPath));
                }

                if (!string.IsNullOrEmpty(videoThumbnailerSetting.WorkingPath))
                {
                    if (!Directory.Exists(videoThumbnailerSetting.WorkingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoThumbnailerSetting.Name,
                    		              videoThumbnailerSetting.WorkingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(videoThumbnailerSetting.WorkingPath));
                    }
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                }

                if (!string.IsNullOrEmpty(videoThumbnailerSetting.ErrorProcessingPath))
                {
                    if (!Directory.Exists(videoThumbnailerSetting.ErrorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoThumbnailerSetting.Name,
                    		              videoThumbnailerSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(videoThumbnailerSetting.ErrorProcessingPath));
                    }
                }

                videoThumbnailerSettingsKeys.Remove(videoThumbnailerSetting.Name);
            }

            if (videoThumbnailerSettingsKeys.Count > 0)
            {
                FileMatchElement fileMatch = null;
                foreach (var value in videoThumbnailerSettingsKeys.Values)
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
