namespace Talifun.Commander.Command.Video.Command.Watermark
{
	public interface IWatermarkSettings
	{
		string Path { get; }
		Gravity Gravity { get; }
		int WidthPadding { get; }
		int HeightPadding { get; }
	}
}
