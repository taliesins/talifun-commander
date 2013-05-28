using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Video
{
    public enum VideoConversionType
    {
		[LocalizableDescription("VideoConversionType_NotSpecified", typeof(Properties.Resource))]
        NotSpecified,
		[LocalizableDescription("VideoConversionType_Flv", typeof(Properties.Resource))]
        Flv,
		[LocalizableDescription("VideoConversionType_H264", typeof(Properties.Resource))]
        H264,
		[LocalizableDescription("VideoConversionType_Vpx8", typeof(Properties.Resource))]
        Vpx8,
        //[LocalizableDescription("VideoConversionType_Vpx9", typeof(Properties.Resource))]
        //Vpx9,
		[LocalizableDescription("VideoConversionType_Theora", typeof(Properties.Resource))]
        Theora,
		[LocalizableDescription("VideoConversionType_Xvid", typeof(Properties.Resource))]
        Xvid,
    }
}