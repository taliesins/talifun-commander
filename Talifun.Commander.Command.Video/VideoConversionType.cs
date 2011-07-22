using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video
{
    public enum VideoConversionType
    {
		[DisplayString(ResourceKey = "VideoConversionType_NotSpecified")]
        NotSpecified,
		[DisplayString(ResourceKey = "VideoConversionType_FLV")]
        FLV,
		[DisplayString(ResourceKey = "VideoConversionType_H264")]
        H264
    }
}