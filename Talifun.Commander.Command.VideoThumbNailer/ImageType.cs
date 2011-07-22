using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbnailer
{
    public enum ImageType
    {
		[DisplayString(ResourceKey = "ImageType_JPG")]
        JPG,
		[DisplayString(ResourceKey = "ImageType_PNG")]
        PNG
    }
}