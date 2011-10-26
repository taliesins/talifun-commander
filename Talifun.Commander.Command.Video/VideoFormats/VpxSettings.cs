using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.VideoFormats
{
	public class VpxSettings : IVideoSettings
	{
		const string AllFixedOptions = @"-threads 0";
		const string FirstPhaseFixedOptions = AllFixedOptions;

		public VpxSettings(VideoConversionElement videoConversion)
		{
			CodecName = "libvpx";
			var maxVideoBitRate = videoConversion.VideoBitRate;
			if (videoConversion.MaxVideoBitRate > 0)
			{
				maxVideoBitRate = videoConversion.MaxVideoBitRate;
			}
			var bufferSize = videoConversion.VideoBitRate * 10;
			if (videoConversion.BufferSize > 0)
			{
				bufferSize = videoConversion.BufferSize;
			}
			var keyframeInterval = videoConversion.FrameRate * 3;
			if (videoConversion.MaxVideoBitRate > 0)
			{
				keyframeInterval = videoConversion.MaxVideoBitRate;
			}
			var minKeyframeInterval = videoConversion.FrameRate;
			if (videoConversion.MinKeyFrameInterval > 0)
			{
				minKeyframeInterval = videoConversion.MinKeyFrameInterval;
			}

			Deinterlace = videoConversion.Deinterlace;
			Width = videoConversion.Width;
			Height = videoConversion.Height;
			AspectRatio = videoConversion.AspectRatio;
			BitRate = videoConversion.VideoBitRate;
			FrameRate = videoConversion.FrameRate;
			MaxBitRate = maxVideoBitRate;
			BufferSize = bufferSize;
			KeyframeInterval = keyframeInterval;
			MinKeyframeInterval = minKeyframeInterval;

			FirstPhaseOptions = FirstPhaseFixedOptions;
			SecondPhaseOptions = string.Empty;
		}

		public string CodecName { get; set; }
		public bool Deinterlace { get; set; } //-deinterlace
		public int Width { get; set; } //-s {Width}x{Height}
		public int Height { get; set; } //-s {Width}x{Height}
		public AspectRatio AspectRatio { get; set; } //-aspect {AspectRatio - 4:3, 16:9, 16:10}
		public int FrameRate { get; set; } //-r {framerate}
		public int BitRate { get; set; } //-b {VideoBitRate}

		public int MaxBitRate { get; set; } //-maxrate {MaxVideoBitRate}
		public int BufferSize { get; set; } //-bufsize {BufferSize}
		public int KeyframeInterval { get; set; }//-g {KeyFrameInterval}
		public int MinKeyframeInterval { get; set; }//-keyint_min {KeyframeInterval}

		public string FirstPhaseOptions { get; set; }
		public string SecondPhaseOptions { get; set; }
	}
}
