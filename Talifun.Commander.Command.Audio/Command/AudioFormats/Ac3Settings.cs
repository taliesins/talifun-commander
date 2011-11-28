using Talifun.Commander.Command.Audio.Configuration;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public class Ac3Settings : IAudioSettings
	{
		public Ac3Settings(AudioConversionElement audioConversion)
		{
			CodecName = "ac3";
			FileNameExtension = "ac3";
			BitRate = audioConversion.BitRate;
			Channels = audioConversion.Channel;
			Frequency = audioConversion.Frequency;
		}

		public string CodecName { get; private set; }
		public string FileNameExtension { get; private set; }
		public int BitRate { get; private set; } //-ab {AudioBitRate}
		public int Frequency { get; private set; } //-ar {AudioFrequency}
		public int Channels { get; private set; }//-ac {AudioChannels - 1, 2}
		public string Options { get; private set; }
	}
}
