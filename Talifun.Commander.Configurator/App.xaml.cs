using System;
using System.IO;
using System.Windows;

namespace Talifun.Commander.Configurator
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			if (File.Exists("Talifun.Commander.TestHarness"))
			{
				AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "Talifun.Commander.TestHarness.config");
			}
			else
			{
				AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "Talifun.Commander.Service.config");
			}
		}
	}
}
