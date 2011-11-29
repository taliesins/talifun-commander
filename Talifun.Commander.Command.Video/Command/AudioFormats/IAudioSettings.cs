namespace Talifun.Commander.Command.Video.Command.AudioFormats
{
	public interface IAudioSettings
	{
		string CodecName { get; set; }
		int BitRate { get; set; }
		int Frequency { get; set; }
		int Channels { get; set; }
		string Options { get; set; }
	}
}