using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
	public class AntiVirusElementCollectionPanelDataModel : DataModelBase
	{
		public AntiVirusElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }

		public string McAffeePath
		{
			get { return AppSettings.Settings[AntiVirusConfiguration.Instance.McAfeePathSettingName].Value; }
			set
			{
				if (AppSettings.Settings[AntiVirusConfiguration.Instance.McAfeePathSettingName].Value == value) return;

				OnPropertyChanging("McAffeePath");
				AppSettings.Settings[AntiVirusConfiguration.Instance.McAfeePathSettingName].Value = value;
				OnPropertyChanged("McAffeePath");
			}
		}
	}
}
