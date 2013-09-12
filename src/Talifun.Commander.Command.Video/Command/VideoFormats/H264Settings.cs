using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.Command.VideoFormats
{
    public class H264Settings : IVideoSettings
    {
        const string AllFixedOptions = @"-y -threads 0 -rc_eq ""blurCplx^(1-qComp)"" -flags +mv4+aic+loop -b-pyramid normal -weightb 1 -mixed-refs 1 -8x8dct 1 -fast-pskip 1 -level 30 -qcomp 0.7 -qmin 10 -qmax 51 -qdiff 4 -bf 16 -b_strategy 1 -i_qfactor 0.71 -cmp chroma -me_range 16 -coder 1 -sc_threshold 40 -partitions parti4x4+parti8x8+partp4x4+partp8x8+partb8x8";
		const string FirstPhaseFixedOptions = AllFixedOptions + @" -subq 1 -me_method dia -refs 1 -trellis 0 -direct-pred 1";
		const string SecondPhaseFixedOptions = AllFixedOptions + @" -subq 7 -me_method umh -refs 4 -trellis 1 -direct-pred 3";

		public H264Settings(VideoConversionElement videoConversion)
		{
			CodecName = "libx264";
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
			SecondPhaseOptions = SecondPhaseFixedOptions;
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