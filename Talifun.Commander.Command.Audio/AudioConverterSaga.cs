using System;
using System.IO;
using Talifun.Commander.Command.Audio.AudioFormats;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Audio.InternalMessages;
using Talifun.Commander.Command.Audio.Properties;
using Talifun.Commander.Command.FileMatcher;

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

		private IAudioSettings GetCommandSettings(AudioConversionElement audioConversionSetting)
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

		private ICommand<IAudioSettings> GetCommand(IAudioSettings audioSettings)
		{
			return new AudioCommand();
		}

    	public override void Run(ICommandSagaProperties properties)
        {
			var commandElement = properties.Project.GetElement<AudioConversionElement>(properties.FileMatch, Settings.ElementCollectionSettingName);

			var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, commandElement.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                
            	var commandSettings = GetCommandSettings(commandElement);
            	var command = GetCommand(commandSettings);

				var encodeSucessful = command.Run(commandSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

                if (encodeSucessful)
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