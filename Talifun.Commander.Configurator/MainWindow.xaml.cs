using System;
using System.Configuration;
using System.IO;
using System.Windows;
using Talifun.Commander.Command;

namespace Talifun.Commander.Configurator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.Visibility = Visibility.Collapsed;
			InitializeComponent();

			var configuration = GetCommanderConfiguration();
			var commanderManager = CommanderManagerFactory.Instance.CreateCommandManager(configuration);
			var commanderSectionWindow = commanderManager.GetCommanderSectionWindow();
			commanderSectionWindow.ShowDialog();

			Close();
		}

		private Configuration GetCommanderConfiguration()
		{
			var configFileName = "Talifun.Commander.MediaConversionService.exe.config";

			if (!File.Exists(configFileName))
			{
				configFileName = "Talifun.Commander.TestHarness.exe.config";
			}

			if (!File.Exists(configFileName))
			{
				throw new Exception("No configuration file found");
			}

			var configMap = new ExeConfigurationFileMap
			                	{
			                		ExeConfigFilename = configFileName
			                	};

			return (ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None));
		}
	}
}
