using System.Configuration;
using System.Linq;

namespace Talifun.Commander.Command.Configuration
{
	public class DefaultPaths : IDefaultPaths
	{
		private readonly AppSettingsSection _appSettings;
		private readonly CommanderSection _commanderSettings;
		public DefaultPaths(AppSettingsSection appSettings, CommanderSection commanderSettings)
		{
			_appSettings = appSettings;
			_commanderSettings = commanderSettings;
		}

		private ProjectElement GetCurrentProjectElement(CommanderSection commanderSettings, NamedConfigurationElement namedConfigurationElement)
		{
			var currentConfigurationElementCollection = commanderSettings.Projects
				.Cast<ProjectElement>()
				.Where(x => x.CommandPlugins
					.Where(y => y.Cast<NamedConfigurationElement>()
						.Where(z => z == namedConfigurationElement)
						.Any())
					.Any()
				)
				.First();

			return currentConfigurationElementCollection;
		}

		private string GetValue(string appSettingKey, NamedConfigurationElement namedConfigurationElement)
		{
			var projectName = GetCurrentProjectElement(_commanderSettings, namedConfigurationElement).Name;
			var elementName = namedConfigurationElement.Name;
			return _appSettings.Settings[appSettingKey].Value
				.Replace("%ProjectName%", projectName)
				.Replace("%ElementName%", elementName);
		}

		public string FolderToWatch(NamedConfigurationElement namedConfigurationElement)
		{
			return GetValue("folderToWatch", namedConfigurationElement);
		}

		public string WorkingPath(NamedConfigurationElement commandConfiguration)
		{
			return GetValue("workingPath", commandConfiguration);
		}

		public string ErrorProcessingPath(NamedConfigurationElement namedConfigurationElement)
		{
			return GetValue("errorProcessingPath", namedConfigurationElement);
		}

		public string OutPutPath(NamedConfigurationElement namedConfigurationElement)
		{
			return GetValue("outPutPath", namedConfigurationElement);
		}

		public string CompletedPath(NamedConfigurationElement namedConfigurationElement)
		{
			return GetValue("completedPath", namedConfigurationElement);
		}
	}
}