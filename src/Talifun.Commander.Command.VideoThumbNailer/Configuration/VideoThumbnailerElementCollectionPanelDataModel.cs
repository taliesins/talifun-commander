using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbnailer.Configuration
{
	public class VideoThumbnailerElementCollectionPanelDataModel : DataModelBase
	{
		public VideoThumbnailerElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }

		public string FFMpegPath
		{
			get { return AppSettings.Settings[VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName].Value; }
			set
			{
				if (AppSettings.Settings[VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName].Value == value) return;

				OnPropertyChanging("FFMpegPath");
				AppSettings.Settings[VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName].Value = value;
				OnPropertyChanged("FFMpegPath");
			}
		}
	}
}
