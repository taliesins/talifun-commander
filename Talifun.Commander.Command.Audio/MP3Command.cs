using System;
using System.IO;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Audio
{
    public class MP3Command : ICommand<MP3Settings>
    {
        #region ICommand<MP3Settings> Members

        public bool Run(MP3Settings settings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + ".mp3";
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

            var commandPath = AudioConversionConfiguration.Instance.FFMpegPath;
            var workingDirectory = outputDirectoryPath.FullName;
            var commandArguments = String.Format("-i \"{0}\" -acodec libmp3lame -ab {1} -ar {2} -ac {3} \"{4}\"", inputFilePath.FullName, settings.AudioBitRate, settings.AudioFrequency, settings.AudioChannels, outPutFilePath.FullName);

            var ffmpegHelper = new FfMpegCommandLineExecutor();
            return ffmpegHelper.Execute(workingDirectory, commandPath, commandArguments, out output);
        }

        #endregion
    }
}
