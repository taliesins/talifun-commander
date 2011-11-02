using System;
using System.IO;
using Talifun.Commander.Command.Video.AudioFormats;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Command.Video.Properties;
using Talifun.Commander.Command.Video.VideoFormats;
using Talifun.Commander.Command.Video.WorkFlow;

namespace Talifun.Commander.Command.Video
{
    public class VideoConverterSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return VideoConversionConfiguration.Instance;
            }
        }

		private IAudioSettings GetAudioSettings(VideoConversionElement videoConversionSetting)
		{
			switch (videoConversionSetting.AudioConversionType)
			{
				case AudioConversionType.Aac:
					return new AacSettings(videoConversionSetting);
				case AudioConversionType.Mp3:
					return new Mp3Settings(videoConversionSetting);
				case AudioConversionType.Ac3:
					return new Ac3Settings(videoConversionSetting);
				case AudioConversionType.Vorbis:
					return new VorbisSettings(videoConversionSetting);
				default:
					throw new Exception(Resource.ErrorMessageUnknownAudioConversionType);
			}
		}

		private IVideoSettings GetVideoSettings(VideoConversionElement videoConversionSetting)
		{
			switch (videoConversionSetting.VideoConversionType)
			{
				case VideoConversionType.Flv:
					return new FlvSettings(videoConversionSetting);
				case VideoConversionType.H264:
					return new H264Settings(videoConversionSetting);
				case VideoConversionType.Theora:
					return new TheoraSettings(videoConversionSetting);
				case VideoConversionType.Vpx:
					return new VpxSettings(videoConversionSetting);
				case VideoConversionType.Xvid:
					return new XvidSettings(videoConversionSetting);
				default:
					throw new Exception(Resource.ErrorMessageUnknownVideoConversionType);
			}
		}

		private IWatermarkSettings GetWatermarkSettings(VideoConversionElement videoConversionSetting)
		{
			var watermarkSettings = new WatermarkSettings()
			                        	{
											Gravity = videoConversionSetting.WatermarkGravity,
											Path = videoConversionSetting.WatermarkPath,
											WidthPadding = videoConversionSetting.WatermarkWidthPadding,
											HeightPadding = videoConversionSetting.WatermarkHeightPadding,
			                        	};

			return watermarkSettings;
		}

		private IContainerSettings GetContainerSettings(VideoConversionElement videoConversionSetting)
		{
			if (videoConversionSetting.VideoConversionType == VideoConversionType.NotSpecified)
			{
				videoConversionSetting.VideoConversionType = VideoConversionType.H264;
			}

			if (videoConversionSetting.AudioConversionType == AudioConversionType.NotSpecified)
			{
				switch (videoConversionSetting.VideoConversionType)
				{
					case VideoConversionType.Flv:
						videoConversionSetting.AudioConversionType = AudioConversionType.Mp3;
						break;
					case VideoConversionType.H264:
						videoConversionSetting.AudioConversionType = AudioConversionType.Aac;
						break;
					case VideoConversionType.Theora:
						videoConversionSetting.AudioConversionType = AudioConversionType.Vorbis;
						break;
					case VideoConversionType.Vpx:
						videoConversionSetting.AudioConversionType = AudioConversionType.Vorbis;
						break;
					case VideoConversionType.Xvid:
						videoConversionSetting.AudioConversionType = AudioConversionType.Ac3;
						break;
					default:
						throw new Exception(Resource.ErrorMessageUnknownVideoConversionType);
				}
			}

			var watermarkSettings = GetWatermarkSettings(videoConversionSetting);
			var videoSettings = GetVideoSettings(videoConversionSetting);
			var audioSettings = GetAudioSettings(videoConversionSetting);

			switch (videoConversionSetting.VideoConversionType)
			{
				case VideoConversionType.NotSpecified:
				case VideoConversionType.Flv:
					return new FlvContainerSettings(audioSettings, videoSettings, watermarkSettings);
				case VideoConversionType.H264:
					return new Mp4ContainerSettings(audioSettings, videoSettings, watermarkSettings);
				case VideoConversionType.Theora:
					return new OggContainerSettings(audioSettings, videoSettings, watermarkSettings);
				case VideoConversionType.Vpx:
					return new WebmContainerSettings(audioSettings, videoSettings, watermarkSettings);
				case VideoConversionType.Xvid:
					return new AviContainerSettings(audioSettings, videoSettings, watermarkSettings);
				default:
					throw new Exception(Resource.ErrorMessageUnknownVideoConversionType);
			}
		}

		private ICommand<IContainerSettings> GetCommand(IContainerSettings containerSettings)
		{
			if (containerSettings is FlvContainerSettings)
			{
				return new FlvWorkFlow();
			}

			if (containerSettings is Mp4ContainerSettings)
			{
				return new Mp4WorkFlow();
			}

			return string.IsNullOrEmpty(containerSettings.Video.SecondPhaseOptions)
			       	? (ICommand<IContainerSettings>) new OnePassWorkFlow()
					: new TwoPassWorkFlow();
		}

    	public override void Run(ICommandSagaProperties properties)
        {
            var videoConversionSetting = GetSettings<VideoConversionElementCollection, VideoConversionElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, videoConversionSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

				var containerSettings = GetContainerSettings(videoConversionSetting);

				var videoCommand = GetCommand(containerSettings);
				var encodeSucessful = videoCommand.Run(containerSettings, properties.AppSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (encodeSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, videoConversionSetting.FileNameFormat, videoConversionSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, workingFilePath, output, videoConversionSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}