using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.ConfigurationTester
{
    public class VideoConversionSettingsTester : CommandConfigurationTesterBase
    {
        public override string ConversionType
        {
            get
            {
                return VideoConversionSettingConfiguration.ConversionType;
            }
        }

        public override void CheckProjectConfiguration(ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<VideoConversionSettingElementCollection>(VideoConversionSettingConfiguration.CollectionSettingName, project);
            var videoConversionSettings = commandSettings.Settings;

            var videoConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (videoConversionSettingsKeys.Count <= 0)
            {
                return;
            }

            var ffMpegPath = VideoConversionSettingConfiguration.FFMpegPath;

            if (string.IsNullOrEmpty(ffMpegPath))
            {
                throw new Exception("FFMpegPath appSetting Required");
            }

            var flvTool2Path = VideoConversionSettingConfiguration.FlvTool2Path;

            if (string.IsNullOrEmpty(flvTool2Path))
            {
                throw new Exception("FlvTool2Path appSetting Required");
            }

            for (var i = 0; i < videoConversionSettings.Count; i++)
            {
                var videoSetting = videoConversionSettings[i];

                if (!Directory.Exists(videoSetting.OutPutPath))
                {
                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><videoConversionSettings><videoConversionSetting name=\"{1}\"> outPutPath does not exist - {2}",
                            project.Name, videoSetting.Name, videoSetting.OutPutPath));
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
                            string.Format(
                                "<project name=\"{0}\"><videoConversionSettings><videoConversionSetting name=\"{1}\"> workingPath does not exist - {2}",
                                project.Name, videoSetting.Name, videoSetting.WorkingPath));
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
                            string.Format(
                                "<project name=\"{0}\"><videoConversionSettings><videoConversionSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                project.Name, videoSetting.Name, videoSetting.ErrorProcessingPath));
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
                        "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <videoConversionSetting> - {2}",
                        project.Name, fileMatch.Name, fileMatch.CommandSettingsKey));
            }
        }
    }
}
