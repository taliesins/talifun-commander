using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.BoxNetUploader.Configuration
{
	public class BoxNetUploaderElementCollectionPanelDataModel : DataModelBase
	{
		public BoxNetUploaderElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }
	}
}
