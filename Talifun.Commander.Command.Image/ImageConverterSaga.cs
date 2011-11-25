using System;
using System.IO;
using Talifun.Commander.Command.FileMatcher;
using Talifun.Commander.Command.Image.Configuration;

namespace Talifun.Commander.Command.Image
{
    public class ImageConverterSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return ImageConversionConfiguration.Instance;
            }
        }

        private IImageResizeSettings GetCommandSettings(ImageConversionElement imageConversion)
        {
            var imageResizeSettings = new ImageResizeSettings
                                          {
                                              BackgroundColour = imageConversion.BackgroundColor,
                                              Gravity = imageConversion.Gravity,
                                              ResizeMode = imageConversion.ResizeMode,
                                              ResizeImageType = imageConversion.ResizeImageType,
                                              Quality = imageConversion.Quality,
											  WatermarkPath = imageConversion.WatermarkPath,
											  WatermarkDissolveLevels = imageConversion.WatermarkDissolveLevels,
											  WatermarkGravity = imageConversion.WatermarkGravity,
                                          };

            if (imageConversion.Height != 0)
            {
                imageResizeSettings.Height = imageConversion.Height;
            }

            if (imageConversion.Width != 0)
            {
                imageResizeSettings.Width = imageConversion.Width;
            }

            return imageResizeSettings;
        }

		private ICommand<IImageResizeSettings> GetCommand(IImageResizeSettings commandLineSettings)
		{
			return new ImageResizeCommand();
		}

        public override void Run(ICommandSagaProperties properties)
        {
			var commandElement = properties.Project.GetElement<ImageConversionElement>(properties.FileMatch, Settings.ElementCollectionSettingName);

			var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, commandElement.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                
				var commandSettings = GetCommandSettings(commandElement);
            	var command = GetCommand(commandSettings);

				var encodeSuccessful = command.Run(commandSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

                if (encodeSuccessful)
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