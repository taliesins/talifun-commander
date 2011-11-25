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
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, videoThumbnailerSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                

                var thumbnailCreationSucessful = false;

                var thumbnailerSettings = GetThumbnailerSettings(videoThumbnailerSetting);
                var thumbnailerCommand = new ThumbnailerCommand();
				thumbnailCreationSucessful = thumbnailerCommand.Run(thumbnailerSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

                if (thumbnailCreationSucessful)
                {
					inputFilePath.MoveCompletedFileToOutputFolder(videoThumbnailerSetting.FileNameFormat, videoThumbnailerSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, inputFilePath, output, videoThumbnailerSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
				workingDirectoryPath.Cleanup();
            }
        }
    }
}