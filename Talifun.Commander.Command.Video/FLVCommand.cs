using System;
using System.IO;
using Talifun.Commander.Executor.CommandLine;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video
{
    public class FlvCommand : ICommand<FlvSettings>
    {
        const string AllFixedOptions = @"-async 4 -f flv -deinterlace -y -qcomp 0.7 -refs 7 -cmp +chroma -coder 1 -me_range 16 -sc_threshold 40 -i_qfactor 0.71 -level 30 -qmin 10 -qmax 51 -qdiff 4";

        #region ICommand<FLVCommand,FLVSettings> Members

        public bool Run(FlvSettings settings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + ".flv";
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));
            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

            var soundArgs = string.Format("-acodec libmp3lame -ar {0} -ab {1} -ac {2}", settings.AudioFrequency, settings.AudioBitRate, settings.AudioChannels);
            var fFMpegCommandArguments = string.Format("-i \"{0}\" -vcodec flv -s {1}x{2} -b {3} -maxrate {4} -bufsize {5} -r {6} -g {7} -keyint_min {8} {9} {10} \"{11}\"", inputFilePath.FullName, settings.Width, settings.Height, settings.VideoBitRate, settings.MaxVideoBitRate, settings.BufferSize, settings.FrameRate, settings.KeyframeInterval, settings.MinKeyframeInterval, AllFixedOptions, soundArgs, outPutFilePath.FullName);
            var flvTool2CommandArguments = string.Format("-U \"{0}\"", outPutFilePath.FullName);

            var workingDirectory = outputDirectoryPath.FullName;
            var fFMpegCommandPath = SettingsHelper.FFMpegPath;
            var flvTool2CommandPath = SettingsHelper.FlvTool2Path;
 
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
