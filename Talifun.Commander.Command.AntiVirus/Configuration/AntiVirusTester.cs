using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    public class AntiVirusTester : CommandConfigurationTesterBase
    {
        #region ICommandConfigurationTester Members
        public override ISettingConfiguration Settings
        {
            get
            {
                return AntiVirusConfiguration.Instance;
            }
        }

        public override void CheckProjectConfiguration(ProjectElement project, AppSettingsSection appSettings)
        {
            var commandSettings = new ProjectElementCommand<AntiVirusElementCollection>(Settings.ElementCollectionSettingName, project);
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
                            string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
								project.Name,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName, 
								antiVirusSetting.Name, 
								antiVirusSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(antiVirusSetting.ErrorProcessingPath));
                    }
                }

                switch (antiVirusSetting.VirusScannerType)
                {
                	case VirusScannerType.McAfee:
                	case VirusScannerType.NotSpecified:
                		{
                			var virusScannerPath = appSettings.Settings[AntiVirusConfiguration.Instance.McAfeePathSettingName].Value;

                			if (string.IsNullOrEmpty(virusScannerPath))
                			{
                				throw new Exception(string.Format(Command.Properties.Resource.ErrorMessageAppSettingRequired, AntiVirusConfiguration.Instance.McAfeePathSettingName));
                			}
                		}
                		break;
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
					Command.Properties.Resource.ErrorMessageCommandConversionSettingKeyPointsToNonExistantCommand,
						project.Name, fileMatch.Name, Settings.ElementSettingName, fileMatch.CommandSettingsKey));
            }
        }

        #endregion
    }
}
