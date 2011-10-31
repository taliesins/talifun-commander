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

		public override void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
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

            	var outPutPath = videoSetting.GetOutPutPathOrDefault();
				if (!Directory.Exists(outPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              videoSetting.Name,
									  outPutPath));
                }
                else
                {
					TryCreateTestFile(new DirectoryInfo(outPutPath));
                }

            	var workingPath = videoSetting.GetWorkingPathOrDefault();
				if (!string.IsNullOrEmpty(workingPath))
                {
					if (!Directory.Exists(workingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoSetting.Name,
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

            	var errorProcessingPath = videoSetting.GetErrorProcessingPathOrDefault();
				if (!string.IsNullOrEmpty(errorProcessingPath))
                {
					if (!Directory.Exists(errorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoSetting.Name,
										  errorProcessingPath));
                    }
                    else
                    {
						TryCreateTestFile(new DirectoryInfo(errorProcessingPath));
                    }
                }

            	var watermarkPath = videoSetting.WatermarkPath;
				if (!string.IsNullOrEmpty(watermarkPath))
				{
					if (!File.Exists(watermarkPath))
					{
						throw new Exception(
							string.Format(Properties.Resource.ErrorMessageCommandWatermarkPathDoesNotExist,
							              project.Name,
							              Settings.ElementCollectionSettingName,
							              Settings.ElementSettingName,
							              videoSetting.Name,
										  watermarkPath));
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
