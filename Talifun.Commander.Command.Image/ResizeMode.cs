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
        AreaToFit,
        /// <summary>
        /// Aka "pan'n'scan' or cut to fit. Aspect is maintained; we show as much as we can, and chop off the overspill.
        /// </summary>
        CutToFit,
        /// <summary>
        /// Aka "letterboxing" or pad out image . Aspect ratio is maintained. We shrink the content so it fits in the frame with no cropping, and pad the rest with a
        /// specified background colour.
        /// </summary>
        Zoom,
        /// <summary>
        /// The output will be scaled to match the target width exactly. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
        FitWidth,
        /// <summary>
        /// The output will be scaled to minimum width if width is smaller than minimum width. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
        FitMinimumWidth,
        /// <summary>
        /// The output will be scaled to maximum width if width is bigger than maximum width. Aspect is maintained. The height of the output is determined by the 
        /// dimensions of the input.
        /// </summary>
        FitMaximumWidth,
        /// <summary>
        /// The output will be scaled to match the target height exactly. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
        FitHeight,
        /// <summary>
        /// The output will be scaled to minimum height if height is smaller than minimum height. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
        FitMinimumHeight,
        /// <summary>
        /// The output will be scaled to maximum height if height is bigger than maximum height. Aspect ratio is maintained; the width of the output is determined by the
        /// dimensions of the input.
        /// </summary>
        FitMaximumHeight,
        /// <summary>
        /// The input is distorted to fit the target frame exactly.
        /// </summary>
        Stretch,
        /// <summary>
        /// The output matches the dimensions of the input; no resizing is performed.
        /// </summary>
        /// <remarks>
        /// Use this to convert image types without resizing
        /// </remarks>
        None
    }
}
