using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.PicasaUploader.Configuration
{
	public class PicasaUploaderElementCollectionPanelDataModel : DataModelBase
	{
		public PicasaUploaderElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }
	}
}
