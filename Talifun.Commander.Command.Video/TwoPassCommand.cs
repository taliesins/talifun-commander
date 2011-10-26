using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video
{
	public class TwoPassCommand : ICommand<IContainerSettings>
    {
		public bool Run(IContainerSettings settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
		{
			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + settings.FileNameExtension;
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

            var fileLog = Path.GetFileNameWithoutExtension(inputFilePath.Name) + ".log";
            var logFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileLog));
            if (logFilePath.Exists)
            {
                logFilePath.Delete();
            }

			var videoFilterArgs = string.Empty;
			if (!string.IsNullOrEmpty(settings.Watermark.Path))
			{
				var overlayPosition = string.Format(settings.Watermark.Gravity.GetOverlayPosition(), settings.Watermark.WidthPadding, settings.Watermark.HeightPadding);
				var watermarkPath = settings.Watermark.Path.Replace('\\', '/').Replace(":", "\\:");
				videoFilterArgs = string.Format("-vf \"movie='{0}' [watermark]; [in][watermark] overlay={1}\"", watermarkPath, overlayPosition);
			}

            var firstPassAudioArgs = "-an";
			var firstPassVideoArgs = string.Format("-codec:v {0} -s {1}x{2} -b:v {3} -maxrate {4} -bufsize {5} -r {6} -g {7} -keyint_min {8}", settings.Video.CodecName, settings.Video.Width, settings.Video.Height, settings.Video.BitRate, settings.Video.MaxBitRate, settings.Video.BufferSize, settings.Video.FrameRate, settings.Video.KeyframeInterval, settings.Video.MinKeyframeInterval);
			var firstPassCommandArguments = string.Format("-i \"{0}\" -passlogfile \"{1}\" -pass 1 {2} {3} {4} \"{5}\"", inputFilePath.FullName, logFilePath.FullName, firstPassVideoArgs, settings.Video.FirstPhaseOptions, firstPassAudioArgs, outPutFilePath.FullName);

			var secondPassAudioArgs = string.Format("-codec:a {0} -b:a {1} -ar {2} -ac {3} {4}", settings.Audio.CodecName, settings.Audio.BitRate, settings.Audio.Frequency, settings.Audio.Channels, settings.Audio.Options);
        	var secondPassVideoArgs = firstPassVideoArgs;
			var secondPassCommandArguments = string.Format("-i \"{0}\" -passlogfile \"{1}\" -pass 2 {2} {3} {4} {5} \"{6}\"", inputFilePath.FullName, logFilePath.FullName, secondPassVideoArgs, settings.Video.SecondPhaseOptions, secondPassAudioArgs, videoFilterArgs, outPutFilePath.FullName);

            var fFMpegCommandPath = appSettings.Settings[VideoConversionConfiguration.Instance.FFMpegPathSettingName].Value;
            var workingDirectory = outputDirectoryPath.FullName;

            var firstPassOutput = string.Empty;
            var secondPassOutput = string.Empty;

            var ffmpegHelper = new FfMpegCommandLineExecutor();
            var result = ffmpegHelper.Execute(workingDirectory, fFMpegCommandPath, firstPassCommandArguments, out firstPassOutput);
            output = firstPassOutput;

            if (result)
            {
                result = ffmpegHelper.Execute(workingDirectory, fFMpegCommandPath, secondPassCommandArguments, out secondPassOutput);
                output += Environment.NewLine + secondPassOutput;
            }

            return result;
        }
    }
}
