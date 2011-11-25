using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Audio.AudioFormats;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Audio
{
	public class AudioCommand : ICommand<IAudioSettings>
	{
		public bool Run(IAudioSettings settings, IDictionary<string, string> appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
		{
			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + settings.FileNameExtension;
			outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var commandPath = appSettings[AudioConversionConfiguration.Instance.FFMpegPathSettingName];
			var workingDirectory = outputDirectoryPath.FullName;

			var commandArguments = String.Format("-i \"{0}\" -y {1} \"{2}\"", inputFilePath.FullName, settings.GetOptions(), outPutFilePath.FullName);

			var ffmpegHelper = new FfMpegCommandLineExecutor();
			return ffmpegHelper.Execute(workingDirectory, commandPath, commandArguments, out output);
		}
	}
}
