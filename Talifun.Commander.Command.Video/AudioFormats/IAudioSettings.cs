namespace Talifun.Commander.Command.Video.AudioFormats
{
	public interface IAudioSettings
	{
		string CodecName { get; set; }
		int BitRate { get; set; }
		int Frequency { get; set; }
		int Channels { get; set; }
	}
}