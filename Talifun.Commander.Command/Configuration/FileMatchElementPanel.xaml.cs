using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Interaction logic for FileMatchElementPanel.xaml
    /// </summary>
    public partial class FileMatchElementPanel : ElementPanelBase
    {
        public FileMatchElementPanel()
        {
            Settings = FileMatchConfiguration.Instance;
            InitializeComponent();

            BindToElement += OnBindToElement;
        }

		private readonly string[] _excludedElements = new[] { "project", "folder", "fileMatch" };

        private FileMatchElement Element { get; set; }
		private Dictionary<string, List<string>> CommandSettings { get; set; }

        private void OnBindToElement(object sender, BindToElementEventArgs e)
        {
            if (e.Element == null || !(e.Element is FileMatchElement)) return;
            Element = e.Element as FileMatchElement;

			var project = GetProjectElement(e.CommanderSettings, Element);
			CommandSettings = GetCommandSettings(CurrentConfiguration.Container, project);

        	var conversionType = Element.ConversionType ?? string.Empty;
        	var commandSettingKey = Element.CommandSettingsKey ?? string.Empty;

			BindConversionTypes(conversionType);
			BindCommandSettingKeys(conversionType, commandSettingKey);
            this.DataContext = Element;
        }

		/// <summary>
		/// Get the project that the file match element belongs too.
		/// </summary>
		/// <param name="commanderSettings"></param>
		/// <param name="fileMatchElement"></param>
		/// <returns></returns>
		private ProjectElement GetProjectElement(CommanderSection commanderSettings, FileMatchElement fileMatchElement)
		{
			var project = commanderSettings.Projects.Cast<ProjectElement>()
				.Where(x => x.Folders.Cast<FolderElement>()
								.Where(y => y.FileMatches.Cast<FileMatchElement>()
												.Where(z => z == fileMatchElement)
												.Any())
								.Any())
				.First();

			return project;
		}

    	/// <summary>
    	/// This is all the command settings that are related to the file match element.
    	/// </summary>
    	/// <param name="container">The container for all the plugins.</param>
    	/// <param name="project">The project to get all the command settings for.</param>
    	/// <returns>Dictionary of command type/command </returns>
    	private Dictionary<string, List<string>> GetCommandSettings(ExportProvider container, ProjectElement project)
		{
			var currentConfigurationElementCollections = container.GetExportedValues<CurrentConfigurationElementCollection>()
				.Where(x => !_excludedElements.Contains(x.Setting.ElementSettingName));

			var commandSettings = new Dictionary<string, List<string>>();

			foreach (var configurationElementCollection in currentConfigurationElementCollections)
			{
				var collectionSettingName = configurationElementCollection.Setting.ElementCollectionSettingName;
				var configurationProperty = project.GetConfigurationProperty(collectionSettingName);
				var commandElementCollection = project.GetCommandConfiguration<CurrentConfigurationElementCollection>(configurationProperty);
				var conversionType = commandElementCollection.Setting.ConversionType;
				var commandSettingKeys = new List<string>();
				for (var i = 0; i < commandElementCollection.Count; i++)
				{
					var commandElement = commandElementCollection[i];
					commandSettingKeys.Add(commandElement.Name);
				}
				commandSettings.Add(conversionType, commandSettingKeys);
			}

			return commandSettings;
		}

		private void BindConversionTypes(string conversionType)
		{
			var conversionTypes = CommandSettings.Select(x => x.Key).OrderBy(x => x);

			var selectableConversionTypes = new List<string>(new[] { string.Empty });
			selectableConversionTypes.AddRange(conversionTypes);

			var selectedConversionType = ((string) conversionTypeComboBox.SelectedValue) ?? string.Empty;
			if (selectedConversionType != conversionType)
			{
				conversionTypeComboBox.SelectedValue = conversionType;
			}

			conversionTypeComboBox.ItemsSource = selectableConversionTypes;
		}

		private void BindCommandSettingKeys(string conversionType, string commandSettingKey)
		{
			var commandSettingKeys = CommandSettings
				.Where(x => x.Key == conversionType)
				.SelectMany(x => x.Value).OrderBy(x => x);

			var selectableCommandSettingKeys = new List<string>(new[] { string.Empty });
			selectableCommandSettingKeys.AddRange(commandSettingKeys);
			

			if (!selectableCommandSettingKeys.Contains(commandSettingKey))
			{
				commandSettingKey = commandSettingKeys.First();
			}

			var selectedCommandSettingKey = ((string) commandSettingKeyComboBox.SelectedValue) ?? string.Empty;
			if (selectedCommandSettingKey != commandSettingKey)
			{
				commandSettingKeyComboBox.SelectedValue = commandSettingKey;
			}

			commandSettingKeyComboBox.ItemsSource = selectableCommandSettingKeys;
		}

		private void ConversionTypeComboBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var conversionType = ((string)conversionTypeComboBox.SelectedValue)??string.Empty;
			var commandSettingKey = ((string) commandSettingKeyComboBox.SelectedValue)??string.Empty;
			BindCommandSettingKeys(conversionType, commandSettingKey);
		}
    }
}