using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.AudioFormats
{
    public class Mp3Settings : IAudioSettings
    {
		public Mp3Settings(VideoConversionElement audioConversion)
		{
			var channels = audioConversion.Channel;

			if (channels > 2)
			{
				channels = 2;
			}

			CodecName = "libmp3lame";
			BitRate = audioConversion.BitRate;
			Channels = channels;
			Frequency = audioConversion.Frequency;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; } 
		public int Frequency { get; set; } 
		public int Channels { get; set; }
	}
}