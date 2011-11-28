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
	}
}