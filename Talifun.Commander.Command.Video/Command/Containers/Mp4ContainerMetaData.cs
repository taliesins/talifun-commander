using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public class Mp4ContainerMetaData : Dictionary<string, string>, IContainerMetaData
	{
		public Mp4ContainerMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{}

		public string GetFfMpegCommandLineArgument()
		{
			var allowedMetaTags = new List<string>
			               	{
			               		"Title",
								"Author",
								"AlbumArtist",
			               		"Album",
			               		"Grouping",
			               		"Composer",
			               		"Year",
								"Track",
			               		"Comment",
								"Genre",
								"Copyright",
								"Description",
								"Synopsis",
								"Show",
								"EpisodeId",
								"Network",
								"Lyrics",
			               	};

			var ffMpegCommandLineArgument = this.Where(x => allowedMetaTags.Contains(x.Key, StringComparer.OrdinalIgnoreCase) && !string.IsNullOrEmpty(x.Value)).Select(x => string.Format("-metadata {0}=\"{1}\"", x.Key, x.Value)).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
			return ffMpegCommandLineArgument.ToString();
		}
	}
}
