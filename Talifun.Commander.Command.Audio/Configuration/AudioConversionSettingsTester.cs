using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio.Configuration
{
    public class AudioConversionSettingsTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return AudioConversionSettingConfiguration.Instance;
            }
        }

        public override void CheckProjectConfiguration(ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<AudioConversionSettingElementCollection>(Settings.CollectionSettingName, project);
            var audioConversionSettings = commandSettings.Settings;

            var audioConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (audioConversionSettingsKeys.Count <= 0)
            {
                return;
            }

            var ffMpegPath = AudioConversionSettingConfiguration.Instance.FFMpegPath;

            if (string.IsNullOrEmpty(ffMpegPath))
            {
                throw new Exception("FFMpegPath appSetting Required");
            }

            
            for (var i = 0; i < audioConversionSettings.Count; i++)
            {
                var audioSetting = audioConversionSettings[i];

                if (!Directory.Exists(audioSetting.OutPutPath))
                {
                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><audioConversionSettings><audioConversionSetting name=\"{1}\"> outPutPath does not exist - {2}",
                            project.Name, audioSetting.Name, audioSetting.OutPutPath));
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
                            string.Format(
                                "<project name=\"{0}\"><audioConversionSettings><audioConversionSetting name=\"{1}\"> workingPath does not exist - {2}",
                                project.Name, audioSetting.Name, audioSetting.WorkingPath));
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
                            string.Format(
                                "<project name=\"{0}\"><audioConversionSettings><audioConversionSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                project.Name, audioSetting.Name, audioSetting.ErrorProcessingPath));
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
                        "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <audioConversionSetting> - {2}",
                        project.Name, fileMatch.Name, fileMatch.CommandSettingsKey));
            }
        }
    }
}
