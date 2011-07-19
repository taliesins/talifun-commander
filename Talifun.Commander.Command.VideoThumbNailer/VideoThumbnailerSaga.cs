﻿using System.IO;
using Talifun.Commander.Command.VideoThumbnailer;
using Talifun.Commander.Command.VideoThumbNailer.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer
{
    public class VideoThumbnailerSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return VideoThumbnailerConfiguration.Instance;
            }
        }

        private ThumbnailerSettings GetThumbnailerSettings(VideoThumbnailerElement videoThumbnailer)
        {
            return new ThumbnailerSettings()
                       {
                           ImageType = videoThumbnailer.ImageType,
                           Width = videoThumbnailer.Width,
                           Height = videoThumbnailer.Height,
                           Time = videoThumbnailer.Time,
                           TimePercentage = videoThumbnailer.TimePercentage
                       };
        }

        public override void Run(ICommandSagaProperties properties)
        {
            var videoThumbnailerSetting = GetSettings<VideoThumbnailerElementCollection, VideoThumbnailerElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, videoThumbnailerSetting.WorkingPath, uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var thumbnailCreationSucessful = false;

                var thumbnailerSettings = GetThumbnailerSettings(videoThumbnailerSetting);
                var thumbnailerCommand = new ThumbnailerCommand();
                thumbnailCreationSucessful = thumbnailerCommand.Run(thumbnailerSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (thumbnailCreationSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, videoThumbnailerSetting.FileNameFormat, videoThumbnailerSetting.OutPutPath);
                }
                else
                {
                    HandleError(output, properties, videoThumbnailerSetting.ErrorProcessingPath, uniqueProcessingNumber);
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}