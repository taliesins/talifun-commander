using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Audio.AudioFormats;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Audio
{
	public class AudioCommand : ICommand<IAudioSettings>
	{
		public bool Run(IAudioSettings settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
		{
			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + settings.FileNameExtension;
			outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var commandPath =appSettings.Settings[AudioConversionConfiguration.Instance.FFMpegPathSettingName].Value;
			var workingDirectory = outputDirectoryPath.FullName;

			var audioArgs = string.Format("-codec:a {0} -b:a {1} -ar {2} -ac {3} {4}", settings.CodecName, settings.BitRate, settings.Frequency, settings.Channels, settings.Options);
			var commandArguments = String.Format("-i \"{0}\" {1} \"{2}\"", inputFilePath.FullName, audioArgs, outPutFilePath.FullName);

			var ffmpegHelper = new FfMpegCommandLineExecutor();
			return ffmpegHelper.Execute(workingDirectory, commandPath, commandArguments, out output);
		}
	}
}
