namespace Talifun.Commander.Command.Image
{
	public interface IImageResizeSettings
	{
		/// <summary>
		/// The out image type to use
		/// </summary>
		ResizeImageType ResizeImageType { get; set; }

		ResizeMode ResizeMode { get; set; }
		int Height { get; set; }
		int Width { get; set; }
		string BackgroundColour { get; set; }
		Gravity Gravity { get; set; }
		int? Quality { get; set; }
		string WatermarkPath { get; set; }
		int WatermarkDissolveLevels { get; set; }
		Gravity WatermarkGravity { get; set; }
	}
}