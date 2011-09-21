using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.VideoFormats
{
    public class FlvSettings : IVideoSettings
    {
		const string AllFixedOptions = @"-async 4 -f flv -deinterlace -y -qcomp 0.7 -refs 7 -cmp +chroma -coder 1 -me_range 16 -sc_threshold 40 -i_qfactor 0.71 -level 30 -qmin 10 -qmax 51 -qdiff 4";

		public FlvSettings(VideoConversionElement videoConversion)
		{
			CodecName = "flv";
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

			FirstPhaseOptions = AllFixedOptions;
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
		public int MinKeyframeInterval { get; set; }//-keyint_min {KeyFrameInterval}

		public string FirstPhaseOptions { get; set; }
		public string SecondPhaseOptions { get; set; }
    }
}