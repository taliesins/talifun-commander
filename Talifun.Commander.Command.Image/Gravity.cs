﻿namespace Talifun.Commander.Command.Image
{
    /// <summary>
    /// Defines positioning options when cropping and compositing images.
    /// </summary>
    public enum Gravity
    {
        /// <summary>
        /// Top of the content should be aligned with the top of the frame, and the content should be horizontally centered in the frame.
        /// </summary>
        North,
        /// <summary>
        /// Top right corner of the content should be aligned with the top right corner of the frame
        /// </summary>
        NorthEast,
        /// <summary>
        /// Content is vertically centered in the frame, with the right edge of the content aligned with the right edge of the frame.
        /// </summary>
        East,
        /// <summary>
        /// Bottom right corner of the content should be aligned with the bottom right corner of the frame.
        /// </summary>
        SouthEast,
        /// <summary>
        /// Content is horizontally centered in the frame, with the bottom of the content aligned with the bottom edge of the frame.
        /// </summary>
        South,
        /// <summary>
        /// Bottom-left corner of the content is aligned with the bottom-left corner of the frame
        /// </summary>
        SouthWest,
        /// <summary>
        /// Content is vertically centered in the frame, with the left edge of the content aligned with the left edge of the frame.
        /// </summary>
        West,
        /// <summary>
        /// Top left corner of the content is aligned with the top left corner of the frame.
        /// </summary>
        NorthWest,
        /// <summary>
        /// The content is centered vertically and horizontally in the frame.
        /// </summary>
        Center
    }
}
