using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.VideoThumbNailer.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.VideoThumbnailer
{
    public class ThumbnailerCommand : ICommand<ThumbnailerSettings>
    {
		const string AllFixedOptions = @"-f image2 -vframes 1";

        #region ICommand<ThumbnailerCommand,ThumbnailerSettings> Members

		public bool Run(ThumbnailerSettings settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            var extension = "";

            switch (settings.ImageType)
            {
                case ImageType.JPG:
                    extension = ".jpg";
                    break;
                case ImageType.PNG:
                    extension = ".png";
                    break;
            }

			
            var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + extension;
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));

            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

            var position = "";

			var commandPath = appSettings.Settings[VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName].Value;
            var videoInfoOutput = string.Empty;
            if (settings.TimePercentage >= 0 && settings.TimePercentage <= 100)
            {
				var videoInfo = VideoInfo.GetVideoInfo(commandPath, inputFilePath, out videoInfoOutput);

                if (videoInfo == null)
                {
                    output = videoInfoOutput;
                    return false;
                }

                var seconds = Convert.ToInt32(Math.Truncate(videoInfo.Duration.TotalSeconds * settings.TimePercentage) / 100);
                var duration = new TimeSpan(0, 0, 0, seconds);

                position = string.Format("-ss {0}", duration.ToString());
            }
            else if (settings.Time != TimeSpan.Zero)
            {
                position = string.Format("-ss {0}", settings.Time.ToString());
            }

            var commandArguments = string.Format("-i \"{0}\" -s {1}x{2} {3} {4} \"{5}\"", inputFilePath.FullName, settings.Width, settings.Height, position, AllFixedOptions, outPutFilePath.FullName);
            var workingDirectory = outputDirectoryPath.FullName;

            var commandOutput = string.Empty;

            var ffmpegHelper = new FfMpegCommandLineExecutor();
            var result = ffmpegHelper.Execute(workingDirectory, commandPath, commandArguments, out commandOutput);

            output = videoInfoOutput + Environment.NewLine + commandOutput;
            return result;
        }

        #endregion
    }
}
