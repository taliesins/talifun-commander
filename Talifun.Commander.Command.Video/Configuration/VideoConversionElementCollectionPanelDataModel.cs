using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video.Configuration
{
	public class VideoConversionElementCollectionPanelDataModel : DataModelBase
	{
		public VideoConversionElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }

		public string FFMpegPath
		{
			get { return AppSettings.Settings[VideoConversionConfiguration.Instance.FFMpegPathSettingName].Value; }
			set
			{
				if (AppSettings.Settings[VideoConversionConfiguration.Instance.FFMpegPathSettingName].Value == value) return;

				OnPropertyChanging("FFMpegPath");
				AppSettings.Settings[VideoConversionConfiguration.Instance.FFMpegPathSettingName].Value = value;
				OnPropertyChanged("FFMpegPath");
			}
		}

		public string FlvTool2Path
		{
			get { return AppSettings.Settings[VideoConversionConfiguration.Instance.FlvTool2PathSettingName].Value; }
			set
			{
				if (AppSettings.Settings[VideoConversionConfiguration.Instance.FlvTool2PathSettingName].Value == value) return;

				OnPropertyChanging("FlvTool2Path");
				AppSettings.Settings[VideoConversionConfiguration.Instance.FlvTool2PathSettingName].Value = value;
				OnPropertyChanged("FlvTool2Path");
			}
		}
	}
}
