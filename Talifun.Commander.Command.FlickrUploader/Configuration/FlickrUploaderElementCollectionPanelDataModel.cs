using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FlickrUploader.Configuration
{
	public class FlickrUploaderElementCollectionPanelDataModel : DataModelBase
	{
		public FlickrUploaderElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }
	}
}
