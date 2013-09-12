namespace Talifun.Commander.Command.Video.Command.VideoFormats
{
	public interface IVideoSettings
	{
		string CodecName { get; set; }
		bool Deinterlace { get; set; }
		int Width { get; set; }
		int Height { get; set; }
		AspectRatio AspectRatio { get; set; }
		int FrameRate { get; set; }
		int BitRate { get; set; }
		int MaxBitRate { get; set; }
		int BufferSize { get; set; }
		int KeyframeInterval { get; set; }
		int MinKeyframeInterval { get; set; }

		string FirstPhaseOptions { get; set; }
		string SecondPhaseOptions { get; set; }
	}
}