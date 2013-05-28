using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.Command.AudioFormats
{
	public class OpusSettings : IAudioSettings
	{
        public OpusSettings(VideoConversionElement audioConversion)
		{
            CodecName = "libopus";
			BitRate = audioConversion.AudioBitRate;
			Channels = audioConversion.AudioChannel;
			Frequency = audioConversion.AudioFrequency;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; }
		public int Frequency { get; set; }
		public int Channels { get; set; }
		public string Options { get; set; }
	}
}
