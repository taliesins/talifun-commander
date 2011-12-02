using System.Collections.Generic;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public class YouTubeMetaData : IVideoMetaData
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Keywords { get; set; }
		public bool Private { get; set; }
		public IList<string> Categories { get; set; }
	}
}
