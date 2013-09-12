using System;

namespace Talifun.Commander.Command.VideoThumbnailer.Command.ThumbnailSettings
{
	public interface IThumbnailerSettings
	{
		/// <summary>
		/// The out image type to use
		/// </summary>
		ImageType ImageType { get; set; }

		/// <summary>
		/// The width of the thumbnail
		/// </summary>
		int Width { get; set; }

		/// <summary>
		/// The height of the thumbnail
		/// </summary>
		int Height { get; set; }

		/// <summary>
		/// The time in the clip to take the thumbnail. Rather use TimePercentage!
		/// </summary>
		TimeSpan Time { get; set; }

		/// <summary>
		/// The percentage into the clip where the thumbnail should be taken. Recommended method to use.
		/// </summary>
		int TimePercentage { get; set; }
	}
}