﻿using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.VideoFormats
{
	public class XvidSettings : IVideoSettings
	{
		const string AllFixedOptions = @"-threads auto";
		const string FirstPhaseFixedOptions = AllFixedOptions;

		public XvidSettings(VideoConversionElement videoConversion)
		{
			CodecName = "libxvid";
			var maxVideoBitRate = videoConversion.VideoBitRate;
			if (videoConversion.MaxVideoBitRate.HasValue)
			{
				maxVideoBitRate = videoConversion.MaxVideoBitRate.Value;
			}
			var bufferSize = videoConversion.VideoBitRate * 10;
			if (videoConversion.BufferSize.HasValue)
			{
				bufferSize = videoConversion.BufferSize.Value;
			}
			var keyframeInterval = videoConversion.FrameRate * 10;
			if (videoConversion.MaxVideoBitRate.HasValue)
			{
				keyframeInterval = videoConversion.MaxVideoBitRate.Value;
			}
			var minKeyframeInterval = videoConversion.FrameRate;
			if (videoConversion.MinKeyframeInterval.HasValue)
			{
				minKeyframeInterval = videoConversion.MinKeyframeInterval.Value;
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

		public int? MaxBitRate { get; set; } //-maxrate {MaxVideoBitRate}
		public int? BufferSize { get; set; } //-bufsize {BufferSize}
		public int? KeyframeInterval { get; set; }//-g {KeyframeInterval}
		public int? MinKeyframeInterval { get; set; }//-keyint_min {KeyframeInterval}

		public string FirstPhaseOptions { get; set; }
		public string SecondPhaseOptions { get; set; }
	}
}
