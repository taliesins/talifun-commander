using System;
using System.IO;
using Talifun.Commander.Command.Image;
using Talifun.Commander.Configuration;

namespace Talifun.Commander.MediaConversion
{
    public class ImageConverterRunner : ICommandRunner<ImageConversionSettingElement>
    {
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

        public void Run(ICommanderManager commanderManager, FileInfo inputFilePath, ImageConversionSettingElement imageConversionSetting)
        {
            var uniqueProcessingNumber = Guid.NewGuid().ToString();
            var uniqueDirectoryName = "image." + inputFilePath.Name + "." + uniqueProcessingNumber;

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(imageConversionSetting.WorkingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(imageConversionSetting.WorkingPath, uniqueDirectoryName));
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

                var encodeSucessful = false;

                var imageResizeSettings = GetImageResizeSettings(imageConversionSetting);
                var imageResizeCommand = new ImageResizeCommand();

                encodeSucessful = imageResizeCommand.Run(imageResizeSettings, inputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (encodeSucessful)
                {
                    var filename = workingFilePath.Name;

                    if (!string.IsNullOrEmpty(imageConversionSetting.FileNameFormat))
                    {
                        filename = string.Format(imageConversionSetting.FileNameFormat, filename);
                    }

                    var outputFilePath = new FileInfo(Path.Combine(imageConversionSetting.OutPutPath, filename));
                    if (outputFilePath.Exists)
                    {
                        outputFilePath.Delete();
                    }

                    workingFilePath.MoveTo(outputFilePath.FullName);
                }
                else
                {
                    FileInfo errorProcessingFilePath = null;
                    if (!string.IsNullOrEmpty(imageConversionSetting.ErrorProcessingPath))
                    {
                        errorProcessingFilePath = new FileInfo(Path.Combine(imageConversionSetting.ErrorProcessingPath, uniqueProcessingNumber + "." + inputFilePath.Name));
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