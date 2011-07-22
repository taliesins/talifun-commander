using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video
{
    public enum AspectRatio
    {
		[DisplayString(ResourceKey = "AspectRatio_NotSpecified")]
        NotSpecified,
		[DisplayString(ResourceKey = "AspectRatio_RatioOf4By3")]
        RatioOf4By3,
		[DisplayString(ResourceKey = "AspectRatio_RatioOf16By9")]
        RatioOf16By9,
		[DisplayString(ResourceKey = "AspectRatio_RatioOf16by10")]
        RatioOf16by10
    }
}