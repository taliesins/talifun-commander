using System.Collections.Generic;

namespace Talifun.Commander.Command.Audio.Command.AudioFormats
{
	public interface IAudioSettings
	{
		string CodecName { get; }
		string FileNameExtension { get; }
		int BitRate { get; }
		int Frequency { get; }
		int Channels { get; }
		string Options { get; }
		List<string> AllowedMetaData { get; set; }
		AudioMetaData MetaData { get; set; }
	}
}