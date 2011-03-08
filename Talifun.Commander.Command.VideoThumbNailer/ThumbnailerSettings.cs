using System;

namespace Talifun.Commander.Command.VideoThumbnailer
{
    public class ThumbnailerSettings
    {
        /// <summary>
        /// The out image type to use
        /// </summary>
        public ImageType ImageType { get; set; } //extension of output filename

        /// <summary>
        /// The width of the thumbnail
        /// </summary>
        public int Width { get; set; } //-s {Width}x{Height}

        /// <summary>
        /// The height of the thumbnail
        /// </summary>
        public int Height { get; set; } //-s {Width}x{Height}

        /// <summary>
        /// The time in the clip to take the thumbnail. Rather use TimePercentage!
        /// </summary>
        public TimeSpan Time { get; set; } //-ss 00:00:00

        /// <summary>
        /// The percentage into the clip where the thumbnail should be taken. Recommended method to use.
        /// </summary>
        public int TimePercentage { get; set; } //used to calculate percentage of time for clip to be used in -ss 00:00:00
    }
}