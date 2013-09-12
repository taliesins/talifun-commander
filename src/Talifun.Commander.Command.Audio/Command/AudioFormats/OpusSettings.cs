using System.Collections.Generic;
using Talifun.Commander.Command.Audio.Configuration;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public class OpusSettings : IAudioSettings
	{
        public OpusSettings(AudioConversionElement audioConversion)
		{
			CodecName = "libopus";
			FileNameExtension = "opus";
			BitRate = audioConversion.BitRate;
			Channels = audioConversion.Channel;
			Frequency = audioConversion.Frequency;

			AllowedMetaData = new List<string>()
			               	{
			               		"Title",
			               		"Author", 
			               		"Album", 
			               		"Year", 
			               		"Comment", 
			               		"Track", 
			               		"Genre",
			               	};
		}

		public string CodecName { get; private set; }
		public string FileNameExtension { get; private set; }
		public int BitRate { get; private set; }
		public int Frequency { get; private set; }
		public int Channels { get; private set; }
		public string Options { get; private set; }
		public List<string> AllowedMetaData { get; set; }
		public AudioMetaData MetaData { get; set; }
	}
}
