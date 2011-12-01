using System.Collections.Generic;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public interface IVideoMetaData
	{
		string Title { get; set; }
		string Description { get; set; }
		string Keywords { get; set; }
		bool Private { get; set; }
		IList<string> Categories { get; set; }
	}
}