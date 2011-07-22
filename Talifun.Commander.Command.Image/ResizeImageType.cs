using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Image
{
    public enum ResizeImageType
    {
		[DisplayString(ResourceKey = "ResizeImageType_JPG")]
        JPG,
		[DisplayString(ResourceKey = "ResizeImageType_PNG")]
        PNG,
		[DisplayString(ResourceKey = "ResizeImageType_GIF")]
        GIF,
		[DisplayString(ResourceKey = "ResizeImageType_Orginal")]
        Orginal
    }
}
