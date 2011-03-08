using System;
using System.IO;
using Talifun.Commander.Command.VideoThumbnailer;
using Talifun.Commander.Configuration;

namespace Talifun.Commander.MediaConversion
{
    public class VideoThumbnailerRunner : ICommandRunner<VideoThumbnailerSettingElement>
    {
        private ThumbnailerSettings GetThumbnailerSettings(VideoThumbnailerSettingElement videoThumbnailerSetting)
        {
            return new ThumbnailerSettings()
                       {
                           ImageType = videoThumbnailerSetting.ImageType,
                           Width = videoThumbnailerSetting.Width,
                           Height = videoThumbnailerSetting.Height,
                           Time = videoThumbnailerSetting.Time,
                           TimePercentage = videoThumbnailerSetting.TimePercentage
                       };
        }

        public void Run(ICommanderManager commanderManager, FileInfo inputFilePath, VideoThumbnailerSettingElement videoThumbnailerSetting)
        {
            var uniqueProcessingNumber = Guid.NewGuid().ToString();
            var uniqueDirectoryName = "video." + inputFilePath.Name + "." + uniqueProcessingNumber;

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(videoThumbnailerSetting.WorkingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(videoThumbnailerSetting.WorkingPath, uniqueDirectoryName));
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

                var thumbnailCreationSucessful = false;

                var thumbnailerSettings = GetThumbnailerSettings(videoThumbnailerSetting);
                var thumbnailerCommand = new ThumbnailerCommand();
                thumbnailCreationSucessful = thumbnailerCommand.Run(thumbnailerSettings, inputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (thumbnailCreationSucessful)
                {
                    var filename = workingFilePath.Name;

                    if (!string.IsNullOrEmpty(videoThumbnailerSetting.FileNameFormat))
                    {
                        filename = string.Format(videoThumbnailerSetting.FileNameFormat, filename);
                    }

                    var outputFilePath = new FileInfo(Path.Combine(videoThumbnailerSetting.OutPutPath, filename));
                    if (outputFilePath.Exists)
                    {
                        outputFilePath.Delete();
                    }

                    workingFilePath.MoveTo(outputFilePath.FullName);
                }
                else
                {
                    FileInfo errorProcessingFilePath = null;
                    if (!string.IsNullOrEmpty(videoThumbnailerSetting.ErrorProcessingPath))
                    {
                        errorProcessingFilePath = new FileInfo(Path.Combine(videoThumbnailerSetting.ErrorProcessingPath, uniqueProcessingNumber + "." + inputFilePath.Name));
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