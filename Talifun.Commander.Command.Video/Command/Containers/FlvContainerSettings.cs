using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;

namespace Talifun.Commander.Command.Video.Command.Containers
{
	public class FlvContainerSettings : IContainerSettings
	{
		public FlvContainerSettings(IAudioSettings audio, IVideoSettings video, IWatermarkSettings watermark)
		{
			FileNameExtension = "flv";
			Audio = audio;

			//These are the only 2 supported audio codecs by flv
			//Flash 9 is required for AAC support
			if (!(Audio.CodecName == "libmp3lame" || Audio.CodecName == "libvo_aacenc"))
			{
				Audio.CodecName = "libmp3lame";
			}

			//Only the following frequencies are supported by flv when using mp3
			if (Audio.CodecName == "libmp3lame")
			{
				if (!(Audio.Frequency == 11025 || Audio.Frequency == 22050 || Audio.Frequency == 44100))
				{
					Audio.Frequency = 22050;
				}
			}

			Video = video;
			Watermark = watermark;
		}

		public string FileNameExtension { get; private set; }
		public string IntroPath { get; set; }
		public string OuttroPath { get; set; }
		public IAudioSettings Audio { get; private set; }
		public IVideoSettings Video { get; private set; }
		public IWatermarkSettings Watermark { get; private set; }
	}
}
