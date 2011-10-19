using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio.Configuration
{
	public class AudioConversionElementCollectionPanelDataModel : DataModelBase
	{
		public AudioConversionElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }

		public string FFMpegPath
		{
			get { return AppSettings.Settings[AudioConversionConfiguration.Instance.FFMpegPathSettingName].Value; }
			set
			{
				if (AppSettings.Settings[AudioConversionConfiguration.Instance.FFMpegPathSettingName].Value == value) return;

				OnPropertyChanging("FFMpegPath");
				AppSettings.Settings[AudioConversionConfiguration.Instance.FFMpegPathSettingName].Value = value;
				OnPropertyChanged("FFMpegPath");
			}
		}
	}
}
