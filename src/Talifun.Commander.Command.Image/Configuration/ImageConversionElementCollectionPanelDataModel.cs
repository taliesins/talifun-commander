using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Image.Configuration
{
	public class ImageConversionElementCollectionPanelDataModel : DataModelBase
	{
		public ImageConversionElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }

		public string ConvertPath
		{
			get { return AppSettings.Settings[ImageConversionConfiguration.Instance.ConvertPathSettingName].Value; }
			set
			{
				if (AppSettings.Settings[ImageConversionConfiguration.Instance.ConvertPathSettingName].Value == value) return;

				OnPropertyChanging("ConvertPath");
				AppSettings.Settings[ImageConversionConfiguration.Instance.ConvertPathSettingName].Value = value;
				OnPropertyChanged("ConvertPath");
			}
		}
	}
}
