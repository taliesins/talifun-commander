using Talifun.Commander.Command.Audio.Configuration;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
    public class Mp3Settings : IAudioSettings
    {
		public Mp3Settings(AudioConversionElement audioConversion)
		{
			var channels = audioConversion.Channel;

			if (channels > 2)
			{
				channels = 2;
			}

			CodecName = "libmp3lame";
			FileNameExtension = "mp3";
			BitRate = audioConversion.BitRate;
			Channels = channels;
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