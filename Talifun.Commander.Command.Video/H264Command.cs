using System;
using System.IO;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video
{
    public class H264Command : ICommand<H264Settings>
    {
        const string AllFixedOptions = @"-y -threads auto -bf 3 -b_strategy 1 -rc_eq ""blurCplx^(1-qComp)""  -qcomp 0.7 -refs 5 -loop 1 -flags +4mv+trell+aic+loop -deblockalpha 0 -deblockbeta 0 -cmp +chroma -coder 1 -me_range 16 -sc_threshold 40 -i_qfactor 0.71 -level 30 -qmin 10 -qmax 51 -qdiff 4";
        const string FirstPhaseFixedOptions = @"-subq 1 -me hex -partitions 0 -trellis 0 -flags2 +mixed_refs";
        const string SecondPhaseFixedOptions = @"-subq 6 -me umh -partitions parti4x4+parti8x8+partp4x4+partp8x8+partb8x8 -flags2 +wpred+mixed_refs+brdo+8x8dct -trellis 1";

        public bool Run(H264Settings settings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + ".mp4";
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

            var firstPassSoundArgs = "-an";
            var firstPassCommandArguments = string.Format("-i \"{0}\" -passlogfile \"{1}\" -pass 1 -vcodec libx264 -s {2}x{3} -b {4} -maxrate {5} -bufsize {6} -r {7} -g {8} -keyint_min {9} {10} {11} {12} -title \"{13}\" \"{14}\"", inputFilePath.FullName, logFilePath.FullName, settings.Width, settings.Height, settings.VideoBitRate, settings.MaxVideoBitRate, settings.BufferSize, settings.FrameRate, settings.KeyframeInterval, settings.MinKeyframeInterval, AllFixedOptions, FirstPhaseFixedOptions, firstPassSoundArgs, outPutFilePath.FullName, outPutFilePath.FullName);

            var secondPassSoundArgs = string.Format("-acodec libfaac -ar {0} -ab {1} -ac {2}", settings.AudioFrequency, settings.AudioBitRate, settings.AudioChannels);
            var secondPassCommandArguments = string.Format("-i \"{0}\" -passlogfile \"{1}\" -pass 2 -vcodec libx264 -s {2}x{3} -b {4} -maxrate {5} -bufsize {6} -r {7} -g {8} -keyint_min {9} {10} {11} {12} -title \"{13}\" \"{14}\"", inputFilePath.FullName, logFilePath.FullName, settings.Width, settings.Height, settings.VideoBitRate, settings.MaxVideoBitRate, settings.BufferSize, settings.FrameRate, settings.KeyframeInterval, settings.MinKeyframeInterval, AllFixedOptions, SecondPhaseFixedOptions, secondPassSoundArgs, outPutFilePath.FullName, outPutFilePath.FullName);

            var fFMpegCommandPath = VideoConversionSettingConfiguration.Instance.FFMpegPath;
            var workingDirectory = outputDirectoryPath.FullName;

            var result = false;
            var firstPassOutput = string.Empty;
            var secondPassOutput = string.Empty;

            var ffmpegHelper = new FfMpegCommandLineExecutor();
            result = ffmpegHelper.Execute(workingDirectory, fFMpegCommandPath, firstPassCommandArguments, out firstPassOutput);
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
