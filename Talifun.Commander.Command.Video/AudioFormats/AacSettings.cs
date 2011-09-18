using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.AudioFormats
{
	public class AacSettings : IAudioSettings
	{
		public AacSettings(VideoConversionElement audioConversion)
		{
			CodecName = "aac";
			BitRate = audioConversion.BitRate;
			Channels = audioConversion.Channel;
			Frequency = audioConversion.Frequency;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; } //-ab {AudioBitRate}
		public int Frequency { get; set; } //-ar {AudioFrequency}
		public int Channels { get; set; }//-ac {AudioChannels - 1, 2}
	}
}
