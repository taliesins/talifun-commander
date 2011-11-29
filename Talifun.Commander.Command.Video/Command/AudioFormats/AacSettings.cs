using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.Command.AudioFormats
{
	public class AacSettings : IAudioSettings
	{
		private const string AllFixedOptions = @"";

		public AacSettings(VideoConversionElement audioConversion)
		{
			CodecName = "libvo_aacenc";
			BitRate = audioConversion.AudioBitRate;
			Channels = audioConversion.AudioChannel;
			Frequency = audioConversion.AudioFrequency;
			Options = AllFixedOptions;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; } //-ab {AudioBitRate}
		public int Frequency { get; set; } //-ar {AudioFrequency}
		public int Channels { get; set; }//-ac {AudioChannels - 1, 2}
		public string Options { get; set; }
	}
}
