using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio
{
    public enum AudioConversionType
    {
		[DisplayString(ResourceKey = "AudioConversionType_NotSpecified")]
        NotSpecified,
		[DisplayString(ResourceKey = "AudioConversionType_MP3")]
        MP3
    }
}