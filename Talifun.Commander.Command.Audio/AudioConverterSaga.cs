using System.IO;
using Talifun.Commander.Command.Audio.Configuration;

namespace Talifun.Commander.Command.Audio
{
    public class AudioConverterSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return AudioConversionSettingConfiguration.Instance;
            }
        }

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

        public override void Run(ICommandSagaProperties properties)
        {
            var audioConversionSetting = GetSettings<AudioConversionSettingElementCollection, AudioConversionSettingElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, audioConversionSetting.WorkingPath, uniqueProcessingNumber);

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
                        encodeSucessful = mp3Command.Run(mp3Settings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                }

                if (encodeSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, audioConversionSetting.FileNameFormat, audioConversionSetting.OutPutPath);
                }
                else
                {
                    HandleError(output, properties, audioConversionSetting.ErrorProcessingPath, uniqueProcessingNumber);
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}