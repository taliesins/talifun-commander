using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Video
{
    public enum VideoConversionType
    {
		[LocalizableDescription("VideoConversionType_NotSpecified", typeof(Properties.Resource))]
        NotSpecified,
		[LocalizableDescription("VideoConversionType_FLV", typeof(Properties.Resource))]
        FLV,
		[LocalizableDescription("VideoConversionType_H264", typeof(Properties.Resource))]
        H264
    }
}