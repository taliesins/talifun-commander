using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.AudioFormats
{
	public class VorbisSettings : IAudioSettings
	{
		public VorbisSettings(VideoConversionElement audioConversion)
		{
			CodecName = "libvorbis";
			BitRate = audioConversion.BitRate;
			Channels = audioConversion.Channel;
			Frequency = audioConversion.Frequency;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; }
		public int Frequency { get; set; }
		public int Channels { get; set; }
	}
}
