using Talifun.Commander.Command.Audio.Configuration;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public class VorbisSettings : IAudioSettings
	{
		public VorbisSettings(AudioConversionElement audioConversion)
		{
			CodecName = "libvorbis";
			FileNameExtension = "oga";
			BitRate = audioConversion.BitRate;
			Channels = audioConversion.Channel;
			Frequency = audioConversion.Frequency;
		}

		public string CodecName { get; private set; }
		public string FileNameExtension { get; private set; }
		public int BitRate { get; private set; }
		public int Frequency { get; private set; }
		public int Channels { get; private set; }
		public string Options { get; private set; }
	}
}
