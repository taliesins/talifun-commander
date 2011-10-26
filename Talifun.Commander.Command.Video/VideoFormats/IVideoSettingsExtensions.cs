using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talifun.Commander.Command.Video.VideoFormats
{
	public static class IVideoSettingsExtensions
	{
		private static StringBuilder GetOptions(IVideoSettings settings)
		{
			var videoOptions = new Dictionary<string, string>()
			                   	{
			                   		{"-codec:v", settings.CodecName}
			                   	};

			if (settings.Width > 0 || settings.Height > 0)
			{
				videoOptions.Add("-s", string.Format("{0}x{1}", settings.Width, settings.Height));
			}

			if (settings.BitRate > 0)
			{
				videoOptions.Add("-b:v", settings.BitRate.ToString());
			}
			if (settings.MaxBitRate > 0)
			{
				videoOptions.Add("-maxrate", settings.MaxBitRate.ToString());
			}
			if (settings.BufferSize > 0)
			{
				videoOptions.Add("-bufsize", settings.BufferSize.ToString());
			}
			if (settings.FrameRate > 0)
			{
				videoOptions.Add("-r", settings.FrameRate.ToString());
			}
			if (settings.KeyframeInterval > 0)
			{
				videoOptions.Add("-g", settings.KeyframeInterval.ToString());
			}
			if (settings.MinKeyframeInterval > 0)
			{
				videoOptions.Add("-keyint_min", settings.MinKeyframeInterval.ToString());
			}

			var value = videoOptions.Select(x=>x.Key + " " + x.Value).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y));

			return value;
		}

		public static string GetOptionsForFirstPass(this IVideoSettings settings)
		{
			var value = GetOptions(settings);
			if (!string.IsNullOrEmpty(settings.FirstPhaseOptions))
			{
				if (value.Length > 0)
				{
					value.Append(" ");
				}
				value.Append(settings.FirstPhaseOptions);
			}

			return value.ToString();
		}

		public static string GetOptionsForSecondPass(this IVideoSettings settings)
		{
			var value = GetOptions(settings);
			if (!string.IsNullOrEmpty(settings.SecondPhaseOptions))
			{
				if (value.Length > 0)
				{
					value.Append(" ");
				}
				value.Append(settings.SecondPhaseOptions);
			}

			return value.ToString();
		}
	}
}
