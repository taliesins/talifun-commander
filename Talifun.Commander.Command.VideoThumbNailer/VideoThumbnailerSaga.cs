using System;
using System.IO;
using Talifun.Commander.Command.FileMatcher;
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
			var uniqueProcessingNumber = Guid.NewGuid().ToString();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, videoThumbnailerSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var thumbnailCreationSucessful = false;

                var thumbnailerSettings = GetThumbnailerSettings(videoThumbnailerSetting);
                var thumbnailerCommand = new ThumbnailerCommand();
                thumbnailCreationSucessful = thumbnailerCommand.Run(thumbnailerSettings, properties.AppSettings, new FileInfo(properties.InputFilePath), workingDirectoryPath, out workingFilePath, out output);

                if (thumbnailCreationSucessful)
                {
					workingFilePath.MoveCompletedFileToOutputFolder(videoThumbnailerSetting.FileNameFormat, videoThumbnailerSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, workingFilePath, output, videoThumbnailerSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
				workingDirectoryPath.Cleanup();
            }
        }
    }
}