using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video
{
	public class OnePassCommand : ICommand<IContainerSettings>
	{
		public bool Run(IContainerSettings settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
		{
			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + settings.FileNameExtension;
			outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var videoFilterArgs = string.Empty;
			if (!string.IsNullOrEmpty(settings.Watermark.Path))
			{
				var overlayPosition = string.Format(settings.Watermark.Gravity.GetOverlayPosition(), settings.Watermark.WidthPadding, settings.Watermark.HeightPadding);
				var watermarkPath = settings.Watermark.Path.Replace('\\', '/').Replace(":", "\\:");
				videoFilterArgs = string.Format("-vf \"movie={0} [watermark]; [in][watermark] overlay={1}\"", watermarkPath, overlayPosition);
			}

			var audioArgs = string.Format("-codec:a {0} -b:a {1} -ar {2} -ac {3} {4}", settings.Audio.CodecName, settings.Audio.BitRate, settings.Audio.Frequency, settings.Audio.Channels, settings.Audio.Options);
			var videoArgs = string.Format("-codec:v {0} -s {1}x{2} -b:v {3} -maxrate {4} -bufsize {5} -r {6} -g {7} -keyint_min {8}", settings.Video.CodecName, settings.Video.Width, settings.Video.Height, settings.Video.BitRate, settings.Video.MaxBitRate, settings.Video.BufferSize, settings.Video.FrameRate, settings.Video.KeyframeInterval, settings.Video.MinKeyframeInterval);

			var fFMpegCommandArguments = string.Format("-i \"{0}\" {1} {2} {3} {4} \"{5}\"", inputFilePath.FullName, videoArgs, settings.Video.FirstPhaseOptions, audioArgs, videoFilterArgs, outPutFilePath.FullName);

			var workingDirectory = outputDirectoryPath.FullName;
			var fFMpegCommandPath = appSettings.Settings[VideoConversionConfiguration.Instance.FFMpegPathSettingName].Value;

			var encodeOutput = string.Empty;

			var ffmpegHelper = new FfMpegCommandLineExecutor();
			var result = ffmpegHelper.Execute(workingDirectory, fFMpegCommandPath, fFMpegCommandArguments, out encodeOutput);
			output = encodeOutput;

			return result;
		}
	}
}
