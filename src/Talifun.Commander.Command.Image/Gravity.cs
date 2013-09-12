using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Image
{
    /// <summary>
    /// Defines positioning options when cropping and compositing images.
    /// </summary>
    public enum Gravity
    {
        /// <summary>
        /// Top of the content should be aligned with the top of the frame, and the content should be horizontally centered in the frame.
        /// </summary>
		[LocalizableDescription("Gravity_North", typeof(Properties.Resource))]
        North,
        /// <summary>
        /// Top right corner of the content should be aligned with the top right corner of the frame
        /// </summary>
		[LocalizableDescription("Gravity_NorthEast", typeof(Properties.Resource))]
        NorthEast,
        /// <summary>
        /// Content is vertically centered in the frame, with the right edge of the content aligned with the right edge of the frame.
        /// </summary>
		[LocalizableDescription("Gravity_East", typeof(Properties.Resource))]
        East,
        /// <summary>
        /// Bottom right corner of the content should be aligned with the bottom right corner of the frame.
        /// </summary>
		[LocalizableDescription("Gravity_SouthEast", typeof(Properties.Resource))]
        SouthEast,
        /// <summary>
        /// Content is horizontally centered in the frame, with the bottom of the content aligned with the bottom edge of the frame.
        /// </summary>
		[LocalizableDescription("Gravity_South", typeof(Properties.Resource))]
        South,
        /// <summary>
        /// Bottom-left corner of the content is aligned with the bottom-left corner of the frame
        /// </summary>
		[LocalizableDescription("Gravity_SouthWest", typeof(Properties.Resource))]
        SouthWest,
        /// <summary>
        /// Content is vertically centered in the frame, with the left edge of the content aligned with the left edge of the frame.
        /// </summary>
		[LocalizableDescription("Gravity_West", typeof(Properties.Resource))]
        West,
        /// <summary>
        /// Top left corner of the content is aligned with the top left corner of the frame.
        /// </summary>
		[LocalizableDescription("Gravity_NorthWest", typeof(Properties.Resource))]
        NorthWest,
        /// <summary>
        /// The content is centered vertically and horizontally in the frame.
        /// </summary>
		[LocalizableDescription("Gravity_Center", typeof(Properties.Resource))]
        Center
    }
}
