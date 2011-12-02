using System;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Image.Command.ImageSettings
{
	public static class IImageResizeSettingsExtensions
	{
		public static string MetaDataArguments(this IImageResizeSettings imageResizeSettings)
		{
			var imageMagickCommandLineArgument = imageResizeSettings.MetaData.Where(x => imageResizeSettings.AllowedMetaData.Contains(x.Key, StringComparer.OrdinalIgnoreCase) && !string.IsNullOrEmpty(x.Value)).Select(x => string.Format("-{0} \"{1}\"", x.Key.ToLower(), x.Value)).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
			return imageMagickCommandLineArgument.ToString();
		}
	}
}
