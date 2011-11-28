using Talifun.Commander.Command.Audio.Configuration;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public class AacSettings : IAudioSettings
	{
		private const string AllFixedOptions = @"";
		public AacSettings(AudioConversionElement audioConversion)
		{
			CodecName = "libvo_aacenc";
			FileNameExtension = "aac";
			BitRate = audioConversion.BitRate;
			Channels = audioConversion.Channel;
			Frequency = audioConversion.Frequency;
			Options = AllFixedOptions;
		}

		public string CodecName { get; private set; }
		public string FileNameExtension { get; private set; }
		public int BitRate { get; private set; } //-ab {AudioBitRate}
		public int Frequency { get; private set; } //-ar {AudioFrequency}
		public int Channels { get; private set; }//-ac {AudioChannels - 1, 2}
		public string Options { get; private set; }
	}
}
