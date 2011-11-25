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

        private IThumbnailerSettings GetCommandSettings(VideoThumbnailerElement videoThumbnailer)
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

		private ICommand<IThumbnailerSettings> GetCommand(IThumbnailerSettings thumbnailerSettings)
		{
			return new ThumbnailerCommand();
		}

        public override void Run(ICommandSagaProperties properties)
        {
			var commandElement = GetSettings<VideoThumbnailerElementCollection, VideoThumbnailerElement>(properties.Project, properties.FileMatch);
			var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, commandElement.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                
				var commandSettings = GetCommandSettings(commandElement);
            	var command = GetCommand(commandSettings);

				var thumbnailCreationSuccessful = command.Run(commandSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

                if (thumbnailCreationSuccessful)
                {
					inputFilePath.MoveCompletedFileToOutputFolder(commandElement.FileNameFormat, commandElement.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, inputFilePath, output, commandElement.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
				workingDirectoryPath.Cleanup();
            }
        }
    }
}