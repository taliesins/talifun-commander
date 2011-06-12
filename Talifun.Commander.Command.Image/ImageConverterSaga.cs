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
                return ImageConversionSettingConfiguration.Instance;
            }
        }

        private ImageResizeSettings GetImageResizeSettings(ImageConversionSettingElement imageConversionSetting)
        {
            var imageResizeSettings = new ImageResizeSettings
                                          {
                                              BackgroundColour = imageConversionSetting.BackgroundColor,
                                              Gravity = imageConversionSetting.Gravity,
                                              ResizeMode = imageConversionSetting.ResizeMode,
                                              ResizeImageType = imageConversionSetting.ResizeImageType,
                                              Quality = imageConversionSetting.Quality
                                          };

            if (imageConversionSetting.Height.HasValue)
            {
                imageResizeSettings.Height = imageConversionSetting.Height.Value;
            }

            if (imageConversionSetting.Width.HasValue)
            {
                imageResizeSettings.Width = imageConversionSetting.Width.Value;
            }


            return imageResizeSettings;
        }

        public override void Run(ICommandSagaProperties properties)
        {
            var imageConversionSetting = GetSettings<ImageConversionSettingElementCollection, ImageConversionSettingElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, imageConversionSetting.WorkingPath, uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var encodeSucessful = false;

                var imageResizeSettings = GetImageResizeSettings(imageConversionSetting);
                var imageResizeCommand = new ImageResizeCommand();

                encodeSucessful = imageResizeCommand.Run(imageResizeSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (encodeSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, imageConversionSetting.FileNameFormat, imageConversionSetting.OutPutPath);
                }
                else
                {
                    HandleError(output, properties, imageConversionSetting.ErrorProcessingPath, uniqueProcessingNumber);
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}