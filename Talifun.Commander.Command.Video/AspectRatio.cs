using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Video
{
    public enum AspectRatio
    {
		[LocalizableDescription("AspectRatio_NotSpecified", typeof(Properties.Resource))]
        NotSpecified,
		[LocalizableDescription("AspectRatio_RatioOf4By3", typeof(Properties.Resource))]
        RatioOf4By3,
		[LocalizableDescription("AspectRatio_RatioOf16By9", typeof(Properties.Resource))]
        RatioOf16By9,
		[LocalizableDescription("AspectRatio_RatioOf16by10", typeof(Properties.Resource))]
        RatioOf16by10
    }
}