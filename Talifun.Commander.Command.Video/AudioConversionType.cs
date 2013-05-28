using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Video
{
	public enum AudioConversionType
	{
		[LocalizableDescription("AudioConversionType_NotSpecified", typeof(Properties.Resource))]
		NotSpecified,
		[LocalizableDescription("AudioConversionType_Mp3", typeof(Properties.Resource))]
		Mp3,
		[LocalizableDescription("AudioConversionType_Ac3", typeof(Properties.Resource))]
		Ac3,
		[LocalizableDescription("AudioConversionType_Aac", typeof(Properties.Resource))]
		Aac,
		[LocalizableDescription("AudioConversionType_Vorbis", typeof(Properties.Resource))]
		Vorbis,
        [LocalizableDescription("AudioConversionType_Opus", typeof(Properties.Resource))]
        Opus,
	}
}
