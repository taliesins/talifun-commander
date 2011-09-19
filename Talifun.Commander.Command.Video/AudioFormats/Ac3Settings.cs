﻿using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.AudioFormats
{
	public class Ac3Settings : IAudioSettings
	{
		public Ac3Settings(VideoConversionElement audioConversion)
		{
			CodecName = "ac3";
			BitRate = audioConversion.AudioBitRate;
			Channels = audioConversion.AudioChannel;
			Frequency = audioConversion.AudioFrequency;
		}

		public string CodecName { get; set; }
		public int BitRate { get; set; } //-ab {AudioBitRate}
		public int Frequency { get; set; } //-ar {AudioFrequency}
		public int Channels { get; set; }//-ac {AudioChannels - 1, 2}
	}
}
