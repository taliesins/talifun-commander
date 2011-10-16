using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video.Configuration
{
    public class VideoConversionTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return VideoConversionConfiguration.Instance;
            }
        }

		public override void CheckProjectConfiguration(ProjectElement project, AppSettingsSection appSettings)
        {
            var commandSettings = new ProjectElementCommand<VideoConversionElementCollection>(Settings.ElementCollectionSettingName, project);
            var videoConversionSettings = commandSettings.Settings;

            var videoConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (videoConversionSettingsKeys.Count <= 0)
            {
                return;
            }

            var ffMpegPath = appSettings.Settings[VideoConversionConfiguration.Instance.FFMpegPathSettingName].Value;

            if (string.IsNullOrEmpty(ffMpegPath))
            {
				throw new Exception(string.Format(Command.Properties.Resource.ErrorMessageAppSettingRequired, VideoConversionConfiguration.Instance.FFMpegPathSettingName));
            }

            var flvTool2Path = appSettings.Settings[VideoConversionConfiguration.Instance.FlvTool2PathSettingName].Value;

            if (string.IsNullOrEmpty(flvTool2Path))
			{
				throw new Exception(string.Format(Command.Properties.Resource.ErrorMessageAppSettingRequired, VideoConversionConfiguration.Instance.FlvTool2PathSettingName));
            }

            for (var i = 0; i < videoConversionSettings.Count; i++)
            {
                var videoSetting = videoConversionSettings[i];

                if (!Directory.Exists(videoSetting.OutPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              videoSetting.Name,
                		              videoSetting.OutPutPath));
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(videoSetting.OutPutPath));
                }

                if (!string.IsNullOrEmpty(videoSetting.WorkingPath))
                {
                    if (!Directory.Exists(videoSetting.WorkingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoSetting.Name,
                    		              videoSetting.WorkingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(videoSetting.WorkingPath));
                    }
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                }

                if (!string.IsNullOrEmpty(videoSetting.ErrorProcessingPath))
                {
                    if (!Directory.Exists(videoSetting.ErrorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoSetting.Name,
                    		              videoSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(videoSetting.ErrorProcessingPath));
                    }
                }

                videoConversionSettingsKeys.Remove(videoSetting.Name);
            }

            if (videoConversionSettingsKeys.Count > 0)
            {
                FileMatchElement fileMatch = null;
                foreach (var value in videoConversionSettingsKeys.Values)
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
