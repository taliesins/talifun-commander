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

		/// <summary>
		/// Get the project element for a plugin.
		/// </summary>
		/// <param name="commanderSettings"></param>
		/// <param name="namedConfigurationElement"></param>
		/// <returns>The project element for the plugin.</returns>
		private ProjectElement GetCurrentProjectElement(CommanderSection commanderSettings, NamedConfigurationElement namedConfigurationElement)
		{
			if (namedConfigurationElement is FolderElement)
			{
				var projectElementForFolderElement = commanderSettings.Projects
					.Where(x => x.Folders
						.Where(y => y.Name == namedConfigurationElement.Name)
						.Any()
					)
					.First();

				return projectElementForFolderElement;
			}

			var projectElementForPlugin = commanderSettings.Projects
				.Where(x => x.CommandPlugins
					.SelectMany(y => y.Cast<NamedConfigurationElement>())					
					.Where(z => z.Name == namedConfigurationElement.Name)
					.Any()
				)
				.First();

			return projectElementForPlugin;
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