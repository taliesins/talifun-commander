﻿using System.IO;
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
                                              Quality = imageConversion.Quality
                                          };

            if (imageConversion.Height.HasValue)
            {
                imageResizeSettings.Height = imageConversion.Height.Value;
            }

            if (imageConversion.Width.HasValue)
            {
                imageResizeSettings.Width = imageConversion.Width.Value;
            }


            return imageResizeSettings;
        }

        public override void Run(ICommandSagaProperties properties)
        {
            var imageConversionSetting = GetSettings<ImageConversionElementCollection, ImageConversionElement>(properties);
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