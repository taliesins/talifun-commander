using System;
using System.IO;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Executor.CommandLine;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video
{
	public class FlvCommand : ICommand<IContainerSettings>
    {
        #region ICommand<FLVCommand,FLVSettings> Members

		public bool Run(IContainerSettings settings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + "." + settings.FileNameExtension;
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

			var audioArgs = string.Format("-acodec {0} -ab {1} -ar {2} -ac {3}", settings.Audio.CodecName, settings.Audio.BitRate, settings.Audio.Frequency, settings.Audio.Channels);
			var videoArgs = string.Format("-vcodec {0} -s {1}x{2} -b {3} -maxrate {4} -bufsize {5} -r {6} -g {7} -keyint_min {8}", settings.Video.CodecName, settings.Video.Width, settings.Video.Height, settings.Video.BitRate, settings.Video.MaxBitRate, settings.Video.BufferSize, settings.Video.FrameRate, settings.Video.KeyframeInterval, settings.Video.MinKeyframeInterval);

			var fFMpegCommandArguments = string.Format("-i \"{0}\" {1} {2} {3} \"{4}\"", inputFilePath.FullName, videoArgs, settings.Video.FirstPhaseOptions, audioArgs, outPutFilePath.FullName);
            var flvTool2CommandArguments = string.Format("-U \"{0}\"", outPutFilePath.FullName);

            var workingDirectory = outputDirectoryPath.FullName;
            var fFMpegCommandPath = VideoConversionConfiguration.Instance.FFMpegPath;
            var flvTool2CommandPath = VideoConversionConfiguration.Instance.FlvTool2Path;
 
            var result = false;
            var encodeOutput = string.Empty;
            var metaDataOutput = string.Empty;

            var ffmpegHelper = new FfMpegCommandLineExecutor();
            result = ffmpegHelper.Execute(workingDirectory, fFMpegCommandPath, fFMpegCommandArguments, out encodeOutput);
            output = encodeOutput;

            if (result)
            {
                var commandLineExecutor = new CommandLineExecutor();
                result = commandLineExecutor.Execute(workingDirectory, flvTool2CommandPath, flvTool2CommandArguments, out metaDataOutput);
                output += Environment.NewLine + metaDataOutput;
            }

            return result;
        }

        #endregion
    }
}
