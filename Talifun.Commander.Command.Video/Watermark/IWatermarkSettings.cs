namespace Talifun.Commander.Command.Video.Watermark
{
	public interface IWatermarkSettings
	{
		string Path { get; }
		Gravity Gravity { get; }
		int WidthPadding { get; }
		int HeightPadding { get; }
	}
}
