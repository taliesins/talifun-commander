using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
    public class VideoThumbnailerSettingsTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return VideoThumbnailerSettingConfiguration.Instance;
            }
        }

        public override void CheckProjectConfiguration(ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<VideoThumbnailerSettingElementCollection>(Settings.CollectionSettingName, project);
            var videoThumbnailerSettings = commandSettings.Settings;

            var videoThumbnailerSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (videoThumbnailerSettingsKeys.Count <= 0)
            {
                return;
            }

            var ffMpegPath = VideoThumbnailerSettingConfiguration.Instance.FFMpegPath;

            if (string.IsNullOrEmpty(ffMpegPath))
            {
                throw new Exception("FFMpegPath appSetting Required");
            }

            for (var i = 0; i < videoThumbnailerSettings.Count; i++)
            {
                var videoThumbnailerSetting = videoThumbnailerSettings[i];

                if (!Directory.Exists(videoThumbnailerSetting.OutPutPath))
                {
                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><videoThumbnailerSettings><videoThumbnailerSetting name=\"{1}\"> outPutPath does not exist - {2}",
                            project.Name, videoThumbnailerSetting.Name, videoThumbnailerSetting.OutPutPath));
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
                            string.Format(
                                "<project name=\"{0}\"><videoThumbnailerSettings><videoThumbnailerSetting name=\"{1}\"> workingPath does not exist - {2}",
                                project.Name, videoThumbnailerSetting.Name, videoThumbnailerSetting.WorkingPath));
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
                            string.Format(
                                "<project name=\"{0}\"><videoThumbnailerSettings><videoThumbnailerSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                project.Name, videoThumbnailerSetting.Name,
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
                        "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <videoThumbnailerSetting> - {2}",
                        project.Name, fileMatch.Name, fileMatch.CommandSettingsKey));
            }
        }
    }
}
