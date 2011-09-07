using Talifun.Commander.UI;

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
		[LocalizableDescription("ResizeMode_AreaToFit", typeof(Properties.Resource))]
        AreaToFit,
        /// <summary>
        /// Aka "pan'n'scan' or cut to fit. Aspect is maintained; we show as much as we can, and chop off the overspill.
        /// </summary>
		[LocalizableDescription("ResizeMode_CutToFit", typeof(Properties.Resource))]
        CutToFit,
        /// <summary>
        /// Aka "letterboxing" or pad out image . Aspect ratio is maintained. We shrink the content so it fits in the frame with no cropping, and pad the rest with a
        /// specified background colour.
        /// </summary>
		[LocalizableDescription("ResizeMode_Zoom", typeof(Properties.Resource))]
        Zoom,
        /// <summary>
        /// The output will be scaled to match the target width exactly. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
		[LocalizableDescription("ResizeMode_FitWidth", typeof(Properties.Resource))]
        FitWidth,
        /// <summary>
        /// The output will be scaled to minimum width if width is smaller than minimum width. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
		[LocalizableDescription("ResizeMode_FitMinimumWidth", typeof(Properties.Resource))]
        FitMinimumWidth,
        /// <summary>
        /// The output will be scaled to maximum width if width is bigger than maximum width. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
		[LocalizableDescription("ResizeMode_FitMaximumWidth", typeof(Properties.Resource))]
        FitMaximumWidth,
        /// <summary>
        /// The output will be scaled to match the target height exactly. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
		[LocalizableDescription("ResizeMode_FitHeight", typeof(Properties.Resource))]
        FitHeight,
        /// <summary>
        /// The output will be scaled to minimum height if height is smaller than minimum height. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
		[LocalizableDescription("ResizeMode_FitMinimumHeight", typeof(Properties.Resource))]
        FitMinimumHeight,
        /// <summary>
        /// The output will be scaled to maximum height if height is bigger than maximum height. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
		[LocalizableDescription("ResizeMode_FitMaximumHeight", typeof(Properties.Resource))]
        FitMaximumHeight,
        /// <summary>
        /// The input is distorted to fit the target frame exactly.
        /// </summary>
		[LocalizableDescription("ResizeMode_Stretch", typeof(Properties.Resource))]
        Stretch,
        /// <summary>
        /// The output matches the dimensions of the input; no resizing is performed.
        /// </summary>
        /// <remarks>
        /// Use this to convert image types without resizing
        /// </remarks>
		[LocalizableDescription("ResizeMode_None", typeof(Properties.Resource))]
        None
    }
}
