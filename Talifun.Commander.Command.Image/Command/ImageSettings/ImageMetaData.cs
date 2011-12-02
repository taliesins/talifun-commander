using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Image.Command.ImageSettings
{
	public class ImageMetaData : Dictionary<string, string>, IImageMetaData
	{
		public ImageMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{}

		public string GetImageMagickCommandLineArgument()
		{
			var allowedMetaTags = new List<string>
			               	{
			               		"Comment",
								"Caption",
			               	};

			var ffMpegCommandLineArgument = this.Where(x=>allowedMetaTags.Contains(x.Key, StringComparer.OrdinalIgnoreCase) && !string.IsNullOrEmpty(x.Value)).Select(x => string.Format("-metadata {0}=\"{1}\"", x.Key, x.Value)).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
			return ffMpegCommandLineArgument.ToString();
		}
	}
}
