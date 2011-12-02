using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public class OggContainerMetaData : Dictionary<string, string>, IContainerMetaData
	{
		public OggContainerMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{}

		public string GetFfMpegCommandLineArgument()
		{
			var ffMpegCommandLineArgument = this.Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => string.Format("-metadata {0}=\"{1}\"", x.Key, x.Value)).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
			return ffMpegCommandLineArgument.ToString();
		}
	}
}
