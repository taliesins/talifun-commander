using System;
using System.IO;
using Talifun.Commander.Command.Audio.AudioFormats;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Audio
{
	public class AudioCommand : ICommand<IAudioSettings>
	{
		public bool Run(IAudioSettings settings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
		{
			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + settings.FileNameExtension;
			outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var commandPath = AudioConversionConfiguration.Instance.FFMpegPath;
			var workingDirectory = outputDirectoryPath.FullName;

			var audioArgs = string.Format("-acodec {0} -ab {1} -ar {2} -ac {3}", settings.CodecName, settings.BitRate, settings.Frequency, settings.Channels);
			var commandArguments = String.Format("-i \"{0}\" {1} \"{2}\"", inputFilePath.FullName, audioArgs, outPutFilePath.FullName);

			var ffmpegHelper = new FfMpegCommandLineExecutor();
			return ffmpegHelper.Execute(workingDirectory, commandPath, commandArguments, out output);
		}
	}
}
