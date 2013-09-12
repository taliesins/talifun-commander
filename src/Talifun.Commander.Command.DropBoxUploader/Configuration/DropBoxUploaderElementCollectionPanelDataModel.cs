using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
	public class DropBoxUploaderElementCollectionPanelDataModel : DataModelBase
	{
		public DropBoxUploaderElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }
	}
}
