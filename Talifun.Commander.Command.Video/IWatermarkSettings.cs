namespace Talifun.Commander.Command.Video
{
	public interface IWatermarkSettings
	{
		string Path { get; }
		Gravity Gravity { get; }
		int WidthPadding { get; }
		int HeightPadding { get; }
	}
}
