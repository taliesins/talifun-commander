using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    public class AntiVirusSettingsTester : CommandConfigurationTesterBase
    {
        #region ICommandConfigurationTester Members
        public override ISettingConfiguration Settings
        {
            get
            {
                return AntiVirusSettingConfiguration.Instance;
            }
        }

        public override void CheckProjectConfiguration(ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<AntiVirusSettingElementCollection>(Settings.CollectionSettingName, project);
            var antiVirusSettings = commandSettings.Settings;

            var antiVirusSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (antiVirusSettings.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < antiVirusSettings.Count; i++)
            {
                var antiVirusSetting = antiVirusSettings[i];

                if (!string.IsNullOrEmpty(antiVirusSetting.ErrorProcessingPath))
                {
                    if (!Directory.Exists(antiVirusSetting.ErrorProcessingPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><antiVirusSettings><antiVirusSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                project.Name, antiVirusSetting.Name, antiVirusSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(antiVirusSetting.ErrorProcessingPath));
                    }
                }

                if (antiVirusSetting.VirusScannerType == VirusScannerType.NotSpecified
                    || antiVirusSetting.VirusScannerType == VirusScannerType.McAfee)
                {
                    var virusScannerPath = AntiVirusSettingConfiguration.Instance.McAfeePath;

                    if (string.IsNullOrEmpty(virusScannerPath))
                    {
                        throw new Exception("McAfeePath appSetting Required");
                    }
                }

                antiVirusSettingsKeys.Remove(antiVirusSetting.Name);
            }

            if (antiVirusSettingsKeys.Count > 0)
            {
                FileMatchElement fileMatch = null;
                foreach (var value in antiVirusSettingsKeys.Values)
                {
                    fileMatch = value;
                    break;
                }

                throw new Exception(
                    string.Format(
                        "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <antiVirusSetting> - {2}",
                        project.Name, fileMatch.Name, fileMatch.CommandSettingsKey));
            }
        }

        #endregion
    }
}
