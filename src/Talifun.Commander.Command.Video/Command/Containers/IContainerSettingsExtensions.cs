using System;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public static class IContainerSettingsExtensions
	{
		public static string MetaDataArguments(this IContainerSettings containerSettings)
		{
			if (containerSettings.AllowedMetaData.Any())
			{
				var ffMpegCommandLineArgument = containerSettings.MetaData.Where(x =>
						containerSettings.AllowedMetaData.Contains(x.Key, StringComparer.OrdinalIgnoreCase) 
						&& !string.IsNullOrEmpty(x.Value))
						.Select(x => string.Format("-metadata {0}=\"{1}\"", x.Key, x.Value))
						.Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
				return ffMpegCommandLineArgument.ToString();
			}
			else
			{
				var ffMpegCommandLineArgument = containerSettings.MetaData.Where(x => 
					!string.IsNullOrEmpty(x.Value))
					.Select(x => string.Format("-metadata {0}=\"{1}\"", x.Key, x.Value))
					.Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
				return ffMpegCommandLineArgument.ToString();				
			}
		}
	}
}
