using System.IO;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video
{
    public class VideoConverterSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return VideoConversionConfiguration.Instance;
            }
        }

        private H264Settings GetH264Settings(VideoConversionElement videoConversion)
        {
            var maxVideoBitRate = videoConversion.VideoBitRate;
            if (videoConversion.MaxVideoBitRate.HasValue)
            {
                maxVideoBitRate = videoConversion.MaxVideoBitRate.Value;
            }
            var bufferSize = videoConversion.VideoBitRate * 10;
            if (videoConversion.BufferSize.HasValue)
            {
                bufferSize = videoConversion.BufferSize.Value;
            }
            var keyframeInterval = videoConversion.FrameRate * 10;
            if (videoConversion.MaxVideoBitRate.HasValue)
            {
                keyframeInterval = videoConversion.MaxVideoBitRate.Value;
            }
            var minKeyframeInterval = videoConversion.FrameRate;
            if (videoConversion.MinKeyframeInterval.HasValue)
            {
                minKeyframeInterval = videoConversion.MinKeyframeInterval.Value;
            }

            var h264Settings = new H264Settings
                                   {
                                       AudioBitRate = videoConversion.BitRate,
                                       AudioChannels = videoConversion.Channel,
                                       AudioFrequency = videoConversion.Frequency,
                                       Deinterlace = videoConversion.Deinterlace,
                                       Width = videoConversion.Width,
                                       Height = videoConversion.Height,
                                       AspectRatio = videoConversion.AspectRatio,
                                       VideoBitRate = videoConversion.VideoBitRate,
                                       FrameRate = videoConversion.FrameRate,
                                       MaxVideoBitRate = maxVideoBitRate,
                                       BufferSize = bufferSize,
                                       KeyframeInterval = keyframeInterval,
                                       MinKeyframeInterval = minKeyframeInterval
                                   };
            return h264Settings;
        }

        private FlvSettings GetFLVSettings(VideoConversionElement videoConversion)
        {
            var maxVideoBitRate = videoConversion.VideoBitRate;
            if (videoConversion.MaxVideoBitRate.HasValue)
            {
                maxVideoBitRate = videoConversion.MaxVideoBitRate.Value;
            }
            var bufferSize = videoConversion.VideoBitRate * 10;
            if (videoConversion.BufferSize.HasValue)
            {
                bufferSize = videoConversion.BufferSize.Value;
            }
            var keyframeInterval = videoConversion.FrameRate * 10;
            if (videoConversion.MaxVideoBitRate.HasValue)
            {
                keyframeInterval = videoConversion.MaxVideoBitRate.Value;
            }
            var minKeyframeInterval = videoConversion.FrameRate;
            if (videoConversion.MinKeyframeInterval.HasValue)
            {
                minKeyframeInterval = videoConversion.MinKeyframeInterval.Value;
            }

            var flvSettings = new FlvSettings
                                  {
                                      AudioBitRate = videoConversion.BitRate,
                                      AudioChannels = videoConversion.Channel,
                                      AudioFrequency = videoConversion.Frequency,
                                      Deinterlace = videoConversion.Deinterlace,
                                      Width = videoConversion.Width,
                                      Height = videoConversion.Height,
                                      AspectRatio = videoConversion.AspectRatio,
                                      VideoBitRate = videoConversion.VideoBitRate,
                                      FrameRate = videoConversion.FrameRate,
                                      MaxVideoBitRate = maxVideoBitRate,
                                      BufferSize = bufferSize,
                                      KeyframeInterval = keyframeInterval,
                                      MinKeyframeInterval = minKeyframeInterval
                                  };
            return flvSettings;
        }

        public override void Run(ICommandSagaProperties properties)
        {
            var videoConversionSetting = GetSettings<VideoConversionElementCollection, VideoConversionElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, videoConversionSetting.WorkingPath, uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                bool encodeSucessful = false;

                switch (videoConversionSetting.VideoConversionType)
                {
                    case VideoConversionType.NotSpecified:
                    case VideoConversionType.FLV:
                        var flvSettings = GetFLVSettings(videoConversionSetting);
                        var flvCommand = new FlvCommand();
                        encodeSucessful = flvCommand.Run(flvSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                    case VideoConversionType.H264:
                        var h264Settings = GetH264Settings(videoConversionSetting);
                        var h264Command = new H264Command();
                        encodeSucessful = h264Command.Run(h264Settings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                }

                if (encodeSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, videoConversionSetting.FileNameFormat, videoConversionSetting.OutPutPath);
                }
                else
                {
                    HandleError(output, properties, videoConversionSetting.ErrorProcessingPath, uniqueProcessingNumber);
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}