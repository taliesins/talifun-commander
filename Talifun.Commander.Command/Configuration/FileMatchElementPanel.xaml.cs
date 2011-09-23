﻿using System.Collections.Generic;
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

        private FileMatchElement Element { get; set; }
		private Dictionary<string, List<string>> CommandSettings { get; set; }

        private void OnBindToElement(object sender, BindToElementEventArgs e)
        {
            if (e.Element == null || !(e.Element is FileMatchElement)) return;
            Element = e.Element as FileMatchElement;

        	var conversionType = Element.ConversionType ?? string.Empty;
        	var commandSettingKey = Element.CommandSettingsKey ?? string.Empty;

			BindConversionTypes(conversionType);
			BindCommandSettingKeys(conversionType, commandSettingKey);
            this.DataContext = Element;
        }

		public void OnBindCommandSettings(Dictionary<string, List<string>> commandSettings)
		{
			CommandSettings = commandSettings;
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