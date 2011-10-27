using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
	public class DefaultPaths : IDefaultPaths
	{
		private readonly AppSettingsSection _appSettings;
		public DefaultPaths(AppSettingsSection appSettings)
		{
			_appSettings = appSettings;
		}

		private string GetValue(string appSettingKey)
		{
			return _appSettings.Settings[appSettingKey].Value;
		}

		public string FolderToWatch(CommandConfigurationBase commandConfiguration)
		{
			return GetValue("folderToWatch");
		}

		public string WorkingPath(CommandConfigurationBase commandConfiguration)
		{
			return GetValue("workingPath");
		}

		public string ErrorProcessingPath(CommandConfigurationBase commandConfiguration)
		{
			return GetValue("errorProcessingPath");
		}

		public string OutPutPath(CommandConfigurationBase commandConfiguration)
		{
			return GetValue("outPutPath");
		}

		public string CompletedPath(CommandConfigurationBase commandConfiguration)
		{
			return GetValue("completedPath");
		}
	}
}