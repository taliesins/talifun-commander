using System;
using System.Collections.Generic;

namespace Talifun.Commander.Command.YouTubeUploader.Command.Settings
{
	public class YouTubeMetaData
	{
		public YouTubeMetaData()
		{
			Title = "Untitled";
			Description = null;
			Credit = null;
			Keywords = null;
			Private = false;
			Updated = null;
			Category = "Shortmov";
			DeveloperTags = new List<string>();
		}

		public string Title { get; set; }
		public string Description { get; set; }
		public string Credit { get; set; }
		public string Keywords { get; set; }
		public bool Private { get; set; }
		public DateTime? Updated { get; set; }
		public string Category { get; set; }
		public IList<string> DeveloperTags { get; set; }
	}
}