using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;

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

		public override void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
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

            	var outPutPath = videoThumbnailerSetting.GetOutPutPathOrDefault();
				if (!Directory.Exists(outPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              videoThumbnailerSetting.Name,
									  outPutPath));
                }
                else
                {
					(new DirectoryInfo(outPutPath)).TryCreateTestFile();
                }

            	var workingPath = videoThumbnailerSetting.GetWorkingPathOrDefault();
				if (!string.IsNullOrEmpty(workingPath))
                {
					if (!Directory.Exists(workingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoThumbnailerSetting.Name,
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

            	var errorProcessingPath = videoThumbnailerSetting.GetErrorProcessingPathOrDefault();
				if (!string.IsNullOrEmpty(errorProcessingPath))
                {
					if (!Directory.Exists(errorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              videoThumbnailerSetting.Name,
										  errorProcessingPath));
                    }
                    else
                    {
						(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
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
