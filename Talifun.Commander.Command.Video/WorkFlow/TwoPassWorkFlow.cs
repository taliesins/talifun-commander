using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Command.Video.VideoFormats;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video.WorkFlow
{
	public class TwoPassWorkFlow : ICommand<IContainerSettings>
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

			var firstPassCommandArguments = string.Format("-i \"{0}\" -y -passlogfile \"{1}\" -pass 1 {2} {3} \"{4}\"", inputFilePath.FullName, logFilePath.FullName, settings.Video.GetOptionsForFirstPass(), "-an", outPutFilePath.FullName);
			var secondPassCommandArguments = string.Format("-i \"{0}\" -y -passlogfile \"{1}\" -pass 2 {2} {3} {4} \"{5}\"", inputFilePath.FullName, logFilePath.FullName, settings.Video.GetOptionsForSecondPass(), settings.Audio.GetOptions(), settings.Watermark.GetOptions(), outPutFilePath.FullName);

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
