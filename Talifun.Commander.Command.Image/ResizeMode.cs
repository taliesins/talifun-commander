using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Image
{
    /// <summary>
    /// Defines various modes for resizing images and photographs to fit a specified frame size 
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// Area to fit. Aspect is maintained. Aspect is maintained; we show as much as we can, and chop off the overspill and pad the rest with a
        /// specified background colour.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_AreaToFit")]
        AreaToFit,
        /// <summary>
        /// Aka "pan'n'scan' or cut to fit. Aspect is maintained; we show as much as we can, and chop off the overspill.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_CutToFit")]
        CutToFit,
        /// <summary>
        /// Aka "letterboxing" or pad out image . Aspect ratio is maintained. We shrink the content so it fits in the frame with no cropping, and pad the rest with a
        /// specified background colour.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_Zoom")]
        Zoom,
        /// <summary>
        /// The output will be scaled to match the target width exactly. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_FitWidth")]
        FitWidth,
        /// <summary>
        /// The output will be scaled to minimum width if width is smaller than minimum width. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_FitMinimumWidth")]
        FitMinimumWidth,
        /// <summary>
        /// The output will be scaled to maximum width if width is bigger than maximum width. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_FitMaximumWidth")]
        FitMaximumWidth,
        /// <summary>
        /// The output will be scaled to match the target height exactly. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_FitHeight")]
        FitHeight,
        /// <summary>
        /// The output will be scaled to minimum height if height is smaller than minimum height. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_FitMinimumHeight")]
        FitMinimumHeight,
        /// <summary>
        /// The output will be scaled to maximum height if height is bigger than maximum height. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_FitMaximumHeight")]
        FitMaximumHeight,
        /// <summary>
        /// The input is distorted to fit the target frame exactly.
        /// </summary>
		[DisplayString(ResourceKey = "ResizeMode_Stretch")]
        Stretch,
        /// <summary>
        /// The output matches the dimensions of the input; no resizing is performed.
        /// </summary>
        /// <remarks>
        /// Use this to convert image types without resizing
        /// </remarks>
		[DisplayString(ResourceKey = "ResizeMode_None")]
        None
    }
}
