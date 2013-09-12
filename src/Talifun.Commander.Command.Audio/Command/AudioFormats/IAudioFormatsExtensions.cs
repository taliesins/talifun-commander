using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public static class IAudioFormatsExtensions
	{
		public static string MetaDataArguments(this IAudioSettings settings)
		{
			var ffMpegCommandLineArgument = settings.MetaData.Where(x => settings.AllowedMetaData.Contains(x.Key, StringComparer.OrdinalIgnoreCase) && !string.IsNullOrEmpty(x.Value)).Select(x => string.Format("-metadata {0}=\"{1}\"", x.Key, x.Value)).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));
			return ffMpegCommandLineArgument.ToString();
		}

		public static string GetOptions(this IAudioSettings settings)
		{
			var audioOptions = new Dictionary<string, string>()
			{
			    {"-codec:a", settings.CodecName}
			};
			if (settings.BitRate > 0)
			{
				audioOptions.Add("-b:a", settings.BitRate.ToString());
			}
			if (settings.Frequency > 0)
			{
				audioOptions.Add("-ar", settings.Frequency.ToString());
			}
			if (settings.Channels > 0)
			{
				audioOptions.Add("-ac", settings.Channels.ToString());
			}

			var value = audioOptions.Select(x => x.Key + " " + x.Value).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));

			if (!string.IsNullOrEmpty(settings.Options))
			{
				if (value.Length > 0)
				{
					value.Append(" ");
				}
				value.Append(settings.Options);
			}

			return value.ToString();
		}
	}
}
