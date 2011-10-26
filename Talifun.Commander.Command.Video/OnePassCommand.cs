using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Command.Video.VideoFormats;
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

			var fFMpegCommandArguments = string.Format("-i \"{0}\" {1} {2} {3} \"{4}\"", inputFilePath.FullName, settings.Video.GetOptionsForFirstPass(), settings.Audio.GetOptions(), settings.Watermark.GetOptions(), outPutFilePath.FullName);

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
