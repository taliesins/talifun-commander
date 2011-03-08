using System;
using System.IO;
using Talifun.Commander.Command.Audio;
using Talifun.Commander.Configuration;

namespace Talifun.Commander.MediaConversion
{
    public class AudioConverterRunner : ICommandRunner<AudioConversionSettingElement>
    {
        private MP3Settings GetMP3Settings(AudioConversionSettingElement audioConversionSetting)
        {
            var mp3Settings = new MP3Settings
                                  {
                                      AudioBitRate = audioConversionSetting.BitRate,
                                      AudioChannels = audioConversionSetting.Channel,
                                      AudioFrequency = audioConversionSetting.Frequency
                                  };
            return mp3Settings;
        }

        public void Run(ICommanderManager commanderManager, FileInfo inputFilePath, AudioConversionSettingElement audioConversionSetting)
        {
            var uniqueProcessingNumber = Guid.NewGuid().ToString();
            var uniqueDirectoryName = "audio." + inputFilePath.Name + "." + uniqueProcessingNumber;

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(audioConversionSetting.WorkingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(audioConversionSetting.WorkingPath, uniqueDirectoryName));
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

                switch (audioConversionSetting.AudioConversionType)
                {
                    case AudioConversionType.NotSpecified:
                    case AudioConversionType.MP3:
                        var mp3Settings = GetMP3Settings(audioConversionSetting);
                        var mp3Command = new MP3Command();
                        encodeSucessful = mp3Command.Run(mp3Settings, inputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                }

                if (encodeSucessful)
                {
                    var filename = workingFilePath.Name;

                    if (!string.IsNullOrEmpty(audioConversionSetting.FileNameFormat))
                    {
                        filename = string.Format(audioConversionSetting.FileNameFormat, filename);
                    }

                    var outputFilePath = new FileInfo(Path.Combine(audioConversionSetting.OutPutPath, filename));
                    if (outputFilePath.Exists)
                    {
                        outputFilePath.Delete();
                    }

                    workingFilePath.MoveTo(outputFilePath.FullName);
                }
                else
                {
                    FileInfo errorProcessingFilePath = null;
                    if (!string.IsNullOrEmpty(audioConversionSetting.ErrorProcessingPath))
                    {
                        errorProcessingFilePath = new FileInfo(Path.Combine(audioConversionSetting.ErrorProcessingPath, uniqueProcessingNumber + "." + inputFilePath.Name));
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