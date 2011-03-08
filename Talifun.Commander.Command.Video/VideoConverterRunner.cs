using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video
{
    public class VideoConverterRunner : ICommandRunner
    {
        public string ConversionType
        {
            get
            {
                return VideoConversionSettingConfiguration.ConversionType;
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

        public void Run(ICommanderManager commanderManager, FileInfo inputFilePath, ProjectElement project, FileMatchElement fileMatch)
        {
            var commandSettings = new ProjectElementCommand<VideoConversionSettingElementCollection>(VideoConversionSettingConfiguration.CollectionSettingName, project);
            var videoConversionSettings = commandSettings.Settings;

            var videoConversionSettingsKey = fileMatch.CommandSettingsKey;
            
            var videoConversionSetting = videoConversionSettings[videoConversionSettingsKey];
            if (videoConversionSetting == null)
                throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                       videoConversionSettingsKey +
                                                       "' does not match any key found in videoConversionSettings name attributes");


            var uniqueProcessingNumber = Guid.NewGuid().ToString();
            var uniqueDirectoryName = "video." + inputFilePath.Name + "." + uniqueProcessingNumber;

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(videoConversionSetting.WorkingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(videoConversionSetting.WorkingPath, uniqueDirectoryName));
            }
            else
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));
            }

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
                        encodeSucessful = flvCommand.Run(flvSettings, inputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                    case VideoConversionType.H264:
                        var h264Settings = GetH264Settings(videoConversionSetting);
                        var h264Command = new H264Command();
                        encodeSucessful = h264Command.Run(h264Settings, inputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                }

                if (encodeSucessful)
                {
                    var filename = workingFilePath.Name;

                    if (!string.IsNullOrEmpty(videoConversionSetting.FileNameFormat))
                    {
                        filename = string.Format(videoConversionSetting.FileNameFormat, filename);
                    }

                    var outputFilePath = new FileInfo(Path.Combine(videoConversionSetting.OutPutPath, filename));
                    if (outputFilePath.Exists)
                    {
                        outputFilePath.Delete();
                    }

                    workingFilePath.MoveTo(outputFilePath.FullName);
                }
                else
                {
                    FileInfo errorProcessingFilePath = null;
                    if (!string.IsNullOrEmpty(videoConversionSetting.ErrorProcessingPath))
                    {
                        errorProcessingFilePath = new FileInfo(Path.Combine(videoConversionSetting.ErrorProcessingPath, uniqueProcessingNumber + "." + inputFilePath.Name));
                    }

                    if (errorProcessingFilePath == null)
                    {
                        var exceptionOccurred = new Exception(output);
                        commanderManager.LogException(null, exceptionOccurred);
                    }
                    else
                    {
                        if (errorProcessingFilePath.Exists)
                        {
                            errorProcessingFilePath.Delete();
                        }

                        var errorProcessingLogFilePath = new FileInfo(errorProcessingFilePath.FullName + ".txt");

                        if (errorProcessingLogFilePath.Exists)
                        {
                            errorProcessingLogFilePath.Delete();
                        }

                        var exceptionOccurred = new Exception(output);
                        commanderManager.LogException(errorProcessingLogFilePath, exceptionOccurred);

                        inputFilePath.CopyTo(errorProcessingFilePath.FullName);
                    }
                }
            }
            finally
            {
                if (workingDirectoryPath.Exists)
                {
                    workingDirectoryPath.Delete(true);
                }
            }
        }
    }
}