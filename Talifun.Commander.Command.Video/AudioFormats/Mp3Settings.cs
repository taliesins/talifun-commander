using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.AudioFormats
{
    public class Mp3Settings : IAudioSettings
    {
		public Mp3Settings(VideoConversionElement audioConversion)
		{
			var channels = audioConversion.AudioChannel;

			if (channels > 2)
			{
				channels = 2;
			}

			CodecName = "libmp3lame";
			BitRate = audioConversion.AudioBitRate;
			Channels = channels;
			Frequency = audioConversion.AudioFrequency;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; } 
		public int Frequency { get; set; } 
		public int Channels { get; set; }
	}
}