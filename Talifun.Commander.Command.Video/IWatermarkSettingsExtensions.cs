namespace Talifun.Commander.Command.Video
{
	public static class IWatermarkSettingsExtensions
	{
		public static string GetOptions(this IWatermarkSettings settings)
		{
			var videoFilterArgs = string.Empty;
			if (!string.IsNullOrEmpty(settings.Path))
			{
				var overlayPosition = string.Format(settings.Gravity.GetOverlayPosition(), settings.WidthPadding, settings.HeightPadding);
				var watermarkPath = settings.Path.Replace('\\', '/').Replace(":", "\\:");
				videoFilterArgs = string.Format("-vf \"movie={0} [watermark]; [in][watermark] overlay={1}\"", watermarkPath, overlayPosition);
			}

			return videoFilterArgs;
		}
	}
}
