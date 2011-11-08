using System;
using System.IO;
using Talifun.Commander.Command.Audio.AudioFormats;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Audio.InternalMessages;
using Talifun.Commander.Command.Audio.Properties;

namespace Talifun.Commander.Command.Audio
{
    public class AudioConverterSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return AudioConversionConfiguration.Instance;
            }
        }

		private IAudioSettings GetAudioSettings(AudioConversionElement audioConversionSetting)
		{
			switch (audioConversionSetting.AudioConversionType)
			{
				case AudioConversionType.NotSpecified:
				case AudioConversionType.Mp3:
					return new Mp3Settings(audioConversionSetting);
				case AudioConversionType.Ac3:
					return new Ac3Settings(audioConversionSetting);
				case AudioConversionType.Aac:
					return new AacSettings(audioConversionSetting);
				case AudioConversionType.Vorbis:
					return new VorbisSettings(audioConversionSetting);
				default:
					throw new Exception(Resource.ErrorMessageUnknownAudioConversionType);
			}
		}

    	public override void Run(ICommandSagaProperties properties)
        {
            var audioConversionSetting = GetSettings<AudioConversionElementCollection, AudioConversionElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, audioConversionSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

            	var audioSettings = GetAudioSettings(audioConversionSetting);

            	var audioCommand = new AudioCommand();
				var encodeSucessful = audioCommand.Run(audioSettings, properties.AppSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (encodeSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, audioConversionSetting.FileNameFormat, audioConversionSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, workingFilePath, output, audioConversionSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}