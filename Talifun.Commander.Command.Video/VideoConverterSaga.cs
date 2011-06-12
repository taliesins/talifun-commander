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
                return VideoConversionSettingConfiguration.Instance;
            }
        }

        private H264Settings GetH264Settings(VideoConversionSettingElement videoConversionSetting)
        {
            var maxVideoBitRate = videoConversionSetting.VideoBitRate;
            if (videoConversionSetting.MaxVideoBitRate.HasValue)
            {
                maxVideoBitRate = videoConversionSetting.MaxVideoBitRate.Value;
            }
            var bufferSize = videoConversionSetting.VideoBitRate * 10;
            if (videoConversionSetting.BufferSize.HasValue)
            {
                bufferSize = videoConversionSetting.BufferSize.Value;
            }
            var keyframeInterval = videoConversionSetting.FrameRate * 10;
            if (videoConversionSetting.MaxVideoBitRate.HasValue)
            {
                keyframeInterval = videoConversionSetting.MaxVideoBitRate.Value;
            }
            var minKeyframeInterval = videoConversionSetting.FrameRate;
            if (videoConversionSetting.MinKeyframeInterval.HasValue)
            {
                minKeyframeInterval = videoConversionSetting.MinKeyframeInterval.Value;
            }

            var h264Settings = new H264Settings
                                   {
                                       AudioBitRate = videoConversionSetting.BitRate,
                                       AudioChannels = videoConversionSetting.Channel,
                                       AudioFrequency = videoConversionSetting.Frequency,
                                       Deinterlace = videoConversionSetting.Deinterlace,
                                       Width = videoConversionSetting.Width,
                                       Height = videoConversionSetting.Height,
                                       AspectRatio = videoConversionSetting.AspectRatio,
                                       VideoBitRate = videoConversionSetting.VideoBitRate,
                                       FrameRate = videoConversionSetting.FrameRate,
                                       MaxVideoBitRate = maxVideoBitRate,
                                       BufferSize = bufferSize,
                                       KeyframeInterval = keyframeInterval,
                                       MinKeyframeInterval = minKeyframeInterval
                                   };
            return h264Settings;
        }

        private FlvSettings GetFLVSettings(VideoConversionSettingElement videoConversionSetting)
        {
            var maxVideoBitRate = videoConversionSetting.VideoBitRate;
            if (videoConversionSetting.MaxVideoBitRate.HasValue)
            {
                maxVideoBitRate = videoConversionSetting.MaxVideoBitRate.Value;
            }
            var bufferSize = videoConversionSetting.VideoBitRate * 10;
            if (videoConversionSetting.BufferSize.HasValue)
            {
                bufferSize = videoConversionSetting.BufferSize.Value;
            }
            var keyframeInterval = videoConversionSetting.FrameRate * 10;
            if (videoConversionSetting.MaxVideoBitRate.HasValue)
            {
                keyframeInterval = videoConversionSetting.MaxVideoBitRate.Value;
            }
            var minKeyframeInterval = videoConversionSetting.FrameRate;
            if (videoConversionSetting.MinKeyframeInterval.HasValue)
            {
                minKeyframeInterval = videoConversionSetting.MinKeyframeInterval.Value;
            }

            var flvSettings = new FlvSettings
                                  {
                                      AudioBitRate = videoConversionSetting.BitRate,
                                      AudioChannels = videoConversionSetting.Channel,
                                      AudioFrequency = videoConversionSetting.Frequency,
                                      Deinterlace = videoConversionSetting.Deinterlace,
                                      Width = videoConversionSetting.Width,
                                      Height = videoConversionSetting.Height,
                                      AspectRatio = videoConversionSetting.AspectRatio,
                                      VideoBitRate = videoConversionSetting.VideoBitRate,
                                      FrameRate = videoConversionSetting.FrameRate,
                                      MaxVideoBitRate = maxVideoBitRate,
                                      BufferSize = bufferSize,
                                      KeyframeInterval = keyframeInterval,
                                      MinKeyframeInterval = minKeyframeInterval
                                  };
            return flvSettings;
        }

        public override void Run(ICommandSagaProperties properties)
        {
            var videoConversionSetting = GetSettings<VideoConversionSettingElementCollection, VideoConversionSettingElement>(properties);
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