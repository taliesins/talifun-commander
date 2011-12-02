using System.Collections.Generic;

namespace Talifun.Commander.Command.Image.Command.ImageSettings
{
	public interface IImageMetaData : IDictionary<string, string>
	{
		string GetImageMagickCommandLineArgument();
	}
}
