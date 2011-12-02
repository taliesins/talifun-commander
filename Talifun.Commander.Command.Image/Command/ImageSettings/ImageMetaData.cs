using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Talifun.Commander.Command.Image.Command.ImageSettings
{
	public class ImageMetaData : Dictionary<string, string>
	{
		public ImageMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{
			AllowedMetaData = new List<string>
			               	{
			               		"Comment",
								"Caption",
			               	};
		}

		[JsonIgnore]
		public IList<string> AllowedMetaData { get; private set; }
	}
}
