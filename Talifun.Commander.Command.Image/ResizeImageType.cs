using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Image
{
    public enum ResizeImageType
    {
		[LocalizableDescription("ResizeImageType_JPG", typeof(Properties.Resource))]
        JPG,
		[LocalizableDescription("ResizeImageType_PNG", typeof(Properties.Resource))]
        PNG,
		[LocalizableDescription("ResizeImageType_GIF", typeof(Properties.Resource))]
        GIF,
		[LocalizableDescription("ResizeImageType_Orginal", typeof(Properties.Resource))]
        Orginal
    }
}
