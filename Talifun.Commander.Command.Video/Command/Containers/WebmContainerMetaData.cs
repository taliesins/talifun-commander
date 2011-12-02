using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public class WebmContainerMetaData : Dictionary<string, string>, IContainerMetaData
	{
		public WebmContainerMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{}

		public string GetFfMpegCommandLineArgument()
		{
			var allowedMetaTags = new List<string>
			               	{
			               		"Title",
								"Description",
								"Langauge",
			               	};

			var ffMpegCommandLineArgument = this.Where(x => allowedMetaTags.Contains(x.Key, StringComparer.OrdinalIgnoreCase) && !string.IsNullOrEmpty(x.Value)).Select(x => string.Format("-metadata {0}=\"{1}\"", x.Key, x.Value)).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
			return ffMpegCommandLineArgument.ToString();
		}
	}
}
