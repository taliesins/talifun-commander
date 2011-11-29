using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Video.Command.AudioFormats;
using Talifun.Commander.Command.Video.Command.Containers;
using Talifun.Commander.Command.Video.Command.Events;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.Command.Response;
using Talifun.Commander.Command.Video.Command.VideoFormats;
using Talifun.Commander.Command.Video.Command.Watermark;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Properties;
using log4net;

namespace Talifun.Commander.Command.Video.Command
{
	[Serializable]
	public class VideoConversionSaga: SagaStateMachine<VideoConversionSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(VideoConversionSaga));

		static VideoConversionSaga()
		{
			Define(() =>
			{
				Initially(
					When(VideoConversionRequestEvent)
						.Then((saga, message) =>
						{
							saga.RequestorCorrelationId = message.RequestorCorrelationId;
							saga.AppSettings = message.AppSettings;
							saga.Configuration = message.Configuration;
							saga.FileMatch = message.FileMatch;
							saga.InputFilePath = message.InputFilePath;

							Log.InfoFormat("Started ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
						})
						.Publish((saga, message) => new VideoConversionStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							Prefix = VideoConversionConfiguration.Instance.ConversionType,
							InputFilePath = saga.InputFilePath,
							WorkingDirectoryPath = saga.Configuration.GetWorkingPathOrDefault()
						})
						.TransitionTo(WaitingForCreateTempDirectory)
					);

				During(
					WaitingForCreateTempDirectory,
					When(CreatedTempDirectory)
						.Then((saga, message) =>
						{
							saga.WorkingDirectoryPath = message.WorkingDirectoryPath;

							Log.InfoFormat("Created Temp Directory ({0}) - {1}", saga.CorrelationId, saga.WorkingDirectoryPath);
						})
						.Then((saga, message)=>
						{
						    var commandMessage = saga.GetAudioConversionWorkflowMessage();
							saga.Bus.Publish(commandMessage.GetType(), commandMessage);
						})
						.TransitionTo(WaitingForExecuteVideoConversionWorkflow)
				);

				During(
					WaitingForExecuteVideoConversionWorkflow,
					When(ExecutedVideoConversionWorkflow)
						.Then((saga, message)=>
						{
						    saga.OutPutFilePath = message.OutPutFilePath;
							saga.OutPut = message.Output;

							if (message.EncodeSuccessful)
							{
								Log.InfoFormat("Processed video conversion workflow ({0}) - {1}", saga.CorrelationId, saga.OutPut);

								var moveProcessedFileIntoOutputDirectoryMessage = new MoveProcessedFileIntoOutputDirectoryMessage()
								{
									CorrelationId = saga.CorrelationId,
									OutputFilePath = saga.OutPutFilePath,
									OutputDirectoryPath = saga.Configuration.OutPutPath
								};
								saga.Bus.Publish(moveProcessedFileIntoOutputDirectoryMessage);
								saga.ChangeCurrentState(WaitingForMoveProcessedFileIntoOutputDirectory);
							}
							else
							{
								Log.WarnFormat("Error processing video conversion workflow ({0}) - {1}", saga.CorrelationId, saga.OutPut);

								var moveProcessedFileIntoErrorDirectoryMessage = new MoveProcessedFileIntoErrorDirectoryMessage()
								{
									CorrelationId = saga.CorrelationId,
									OutputFilePath = saga.OutPutFilePath,
									ErrorDirectoryPath = saga.Configuration.ErrorProcessingPath
								};
								saga.Bus.Publish(moveProcessedFileIntoErrorDirectoryMessage);
								saga.ChangeCurrentState(WaitingForMoveProcessedFileIntoErrorDirectory);
							}
						})
				);

				During(
					WaitingForMoveProcessedFileIntoOutputDirectory,
					When(MovedProcessedFileIntoOutputDirectory)
						.Then((saga, message)=>
						{
							Log.InfoFormat("Moved processed file to output directory  ({0}) - {1}", saga.CorrelationId, message.OutputFilePath);
						})
						.Publish((saga, message) => new DeleteTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							WorkingPath = saga.WorkingDirectoryPath
						})
						.TransitionTo(WaitingForDeleteTempDirectory)
				);

				During(
					WaitingForMoveProcessedFileIntoErrorDirectory,
					When(MovedProcessedFileIntoErrorDirectory)
						.Then((saga, message) =>
						{
							Log.InfoFormat("Moved errored file to error directory ({0}) - {1}", saga.CorrelationId, message.OutputFilePath);
						})
						.Publish((saga, message) => new DeleteTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							WorkingPath = saga.WorkingDirectoryPath
						})
						.TransitionTo(WaitingForDeleteTempDirectory)
				);

				During(
					WaitingForDeleteTempDirectory,
					When(DeletedTempDirectory)
						.Then((saga, message)=>
						{
							Log.InfoFormat("Deleted Temp Directory ({0}) - {1} ", saga.CorrelationId, saga.WorkingDirectoryPath);
							Log.InfoFormat("Completed ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
						})
						.Publish((saga, message) => new VideoConversionCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new VideoConversionResponseMessage
						    {
						      	CorrelationId = saga.RequestorCorrelationId,
								ResponderCorrelationId = saga.CorrelationId
						    }
						)
						.Complete()
				);
			});
		}

		// ReSharper disable UnusedMember.Global
		public static State Initial { get; set; }
		// ReSharper restore UnusedMember.Global
		// ReSharper disable UnusedMember.Global
		public static State Completed { get; set; }
		// ReSharper restore UnusedMember.Global

		public VideoConversionSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual VideoConversionElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual string WorkingDirectoryPath { get; set; }
		public virtual string OutPut { get; set; }
		public virtual string OutPutFilePath { get; set; }

		#region Initialise
		public static Event<VideoConversionRequestMessage> VideoConversionRequestEvent { get; set; }
		#endregion

		#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteVideoConversionWorkflow { get; set; }
		public static Event<IExecutedVideoConversionWorkflowMessage> ExecutedVideoConversionWorkflow { get; set; }
		//public static Event<Fault<IExecuteVideoConversionWorkflowMessage, Guid>> ExecuteVideoConversionWorkflowFailed { get; set; }

		private IExecuteVideoConversionWorkflowMessage GetAudioConversionWorkflowMessage()
		{
			var commandSettings = GetCommandSettings(Configuration);
			var commandMessage = GetCommandMessage(commandSettings);

			commandMessage.CorrelationId = CorrelationId;
			commandMessage.AppSettings = AppSettings;
			commandMessage.InputFilePath = InputFilePath;
			commandMessage.WorkingDirectoryPath = WorkingDirectoryPath;
			commandMessage.Settings = commandSettings;
			return commandMessage;
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

		private IContainerSettings GetCommandSettings(VideoConversionElement videoConversionSetting)
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

		private IExecuteVideoConversionWorkflowMessage GetCommandMessage(IContainerSettings containerSettings)
		{
			if (containerSettings is FlvContainerSettings)
			{
				return new ExecuteFlvConversionWorkflowMessage();
			}

			if (containerSettings is Mp4ContainerSettings)
			{
				return new ExecuteMp4ConversionWorkflowMessage();
			}

			return string.IsNullOrEmpty(containerSettings.Video.SecondPhaseOptions)
					? (IExecuteVideoConversionWorkflowMessage)new ExecuteOnePassConversionWorkflowMessage()
					: new ExecuteTwoPassConversionWorkflowMessage();
		}
		#endregion

		#region Move Processed File To Original Directory
		public static State WaitingForMoveProcessedFileIntoOutputDirectory { get; set; }
		public static Event<MovedProcessedFileIntoOutputDirectoryMessage> MovedProcessedFileIntoOutputDirectory { get; set; }
		//public static Event<Fault<MoveProcessedFileIntoOutputDirectoryMessage, Guid>> MoveProcessedFileIntoOutputDirectoryMessageFailed { get; set; }
		#endregion

		#region Move Processed File To Error Directory
		public static State WaitingForMoveProcessedFileIntoErrorDirectory { get; set; }
		public static Event<MovedProcessedFileIntoErrorDirectoryMessage> MovedProcessedFileIntoErrorDirectory { get; set; }
		//public static Event<Fault<MoveProcessedFileIntoErrorDirectoryMessage, Guid>> MoveProcessedFileIntoErrorDirectoryMessageFailed { get; set; }
		#endregion

		#region Delete Temp Directory
		public static State WaitingForDeleteTempDirectory { get; set; }
		public static Event<DeletedTempDirectoryMessage> DeletedTempDirectory { get; set; }
		//public static Event<Fault<DeleteTempDirectoryMessage, Guid>> DeleteTempDirectoryFailed { get; set; }
		#endregion
	}
}
