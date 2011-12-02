using System;
using System.Collections.Generic;

namespace Talifun.Commander.Command.Image.Command.ImageSettings
{
	public class ImageMetaData : Dictionary<string, string>
	{
		public ImageMetaData()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}
	}
}
