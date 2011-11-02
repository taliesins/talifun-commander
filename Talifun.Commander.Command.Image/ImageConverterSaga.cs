using System.IO;
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

        private ImageResizeSettings GetImageResizeSettings(ImageConversionElement imageConversion)
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

        public override void Run(ICommandSagaProperties properties)
        {
            var imageConversionSetting = GetSettings<ImageConversionElementCollection, ImageConversionElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, imageConversionSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var encodeSucessful = false;

                var imageResizeSettings = GetImageResizeSettings(imageConversionSetting);
                var imageResizeCommand = new ImageResizeCommand();

                encodeSucessful = imageResizeCommand.Run(imageResizeSettings, properties.AppSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (encodeSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, imageConversionSetting.FileNameFormat, imageConversionSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, workingFilePath, output, imageConversionSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}