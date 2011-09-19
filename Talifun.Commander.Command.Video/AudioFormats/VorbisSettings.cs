using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.AudioFormats
{
	public class VorbisSettings : IAudioSettings
	{
		public VorbisSettings(VideoConversionElement audioConversion)
		{
			CodecName = "libvorbis";
			BitRate = audioConversion.AudioBitRate;
			Channels = audioConversion.AudioChannel;
			Frequency = audioConversion.AudioFrequency;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; }
		public int Frequency { get; set; }
		public int Channels { get; set; }
	}
}
