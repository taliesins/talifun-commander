using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	public class YouTubeUploaderElementCollectionPanelDataModel : DataModelBase
	{
		public YouTubeUploaderElementCollectionPanelDataModel(AppSettingsSection appSettings)
		{
			AppSettings = appSettings;
		}

		private AppSettingsSection AppSettings { get; set; }
	}
}
