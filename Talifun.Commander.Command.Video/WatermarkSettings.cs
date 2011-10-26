namespace Talifun.Commander.Command.Video
{
	public class WatermarkSettings : IWatermarkSettings
	{
		public string Path { get; set; }
		public Gravity Gravity { get; set; }
		public int WidthPadding { get; set; }
		public int HeightPadding { get; set; }
	}
}
