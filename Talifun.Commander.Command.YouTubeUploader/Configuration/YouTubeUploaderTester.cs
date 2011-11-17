using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	public class YouTubeUploaderTester : CommandConfigurationTesterBase
	{
		public override ISettingConfiguration Settings
		{
			get
			{
				return YouTubeUploaderConfiguration.Instance;
			}
		}

		public override void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
		{
			var commandSettings = new ProjectElementCommand<YouTubeUploaderElementCollection>(Settings.ElementCollectionSettingName, project);
			var youTubeUploaderSettings = commandSettings.Settings;

			var youTubeUploaderSettingsKeys = new Dictionary<string, FileMatchElement>();

			if (youTubeUploaderSettingsKeys.Count <= 0)
			{
				return;
			}

			for (var i = 0; i < youTubeUploaderSettings.Count; i++)
			{
				var youTubeUploaderSetting = youTubeUploaderSettings[i];

				var outPutPath = youTubeUploaderSetting.GetOutPutPathOrDefault();
				if (!Directory.Exists(outPutPath))
				{
					throw new Exception(
						string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
									  project.Name,
									  Settings.ElementCollectionSettingName,
									  Settings.ElementSettingName,
									  youTubeUploaderSetting.Name,
									  outPutPath));
				}
				else
				{
					(new DirectoryInfo(outPutPath)).TryCreateTestFile();
				}

				var workingPath = youTubeUploaderSetting.GetWorkingPathOrDefault();
				if (!string.IsNullOrEmpty(workingPath))
				{
					if (!Directory.Exists(workingPath))
					{
						throw new Exception(
							string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
										  project.Name,
										  Settings.ElementCollectionSettingName,
										  Settings.ElementSettingName,
										  youTubeUploaderSetting.Name,
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

				var errorProcessingPath = youTubeUploaderSetting.GetErrorProcessingPathOrDefault();
				if (!string.IsNullOrEmpty(errorProcessingPath))
				{
					if (!Directory.Exists(errorProcessingPath))
					{
						throw new Exception(
							string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
										  project.Name,
										  Settings.ElementCollectionSettingName,
										  Settings.ElementSettingName,
										  youTubeUploaderSetting.Name,
										  errorProcessingPath));
					}
					else
					{
						(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
					}
				}

				youTubeUploaderSettingsKeys.Remove(youTubeUploaderSetting.Name);
			}

			if (youTubeUploaderSettingsKeys.Count > 0)
			{
				FileMatchElement fileMatch = null;
				foreach (var value in youTubeUploaderSettingsKeys.Values)
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
