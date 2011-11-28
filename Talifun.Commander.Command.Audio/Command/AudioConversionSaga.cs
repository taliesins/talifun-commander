using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Audio.Command.AudioFormats;
using Talifun.Commander.Command.Audio.Command.Events;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.Command.Response;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Audio.Properties;
using Talifun.Commander.Command.Configuration;
using log4net;

namespace Talifun.Commander.Command.Audio.Command
{
	[Serializable]
	public class AudioConversionSaga: SagaStateMachine<AudioConversionSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(AudioConversionSaga));

		static AudioConversionSaga()
		{
			Define(() =>
			{
				Initially(
					When(AntiVirusRequestEvent)
						.Then((saga, message) =>
						{
							saga.RequestorCorrelationId = message.RequestorCorrelationId;
							saga.AppSettings = message.AppSettings;
							saga.Configuration = message.Configuration;
							saga.FileMatch = message.FileMatch;
							saga.InputFilePath = message.InputFilePath;

							Log.InfoFormat("Started ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
						})
						.Publish((saga, message) => new AudioConversionStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							Prefix = AudioConversionConfiguration.Instance.ConversionType,
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
						.TransitionTo(WaitingForExecuteAudioConversionWorkflow)
				);

				During(
					WaitingForExecuteAudioConversionWorkflow,
					When(ExecutedAudioConversionWorkflow)
						.Then((saga, message)=>
						{
						    saga.OutPutFilePath = message.OutPutFilePath;
							saga.OutPut = message.Output;

							if (message.EncodeSuccessful)
							{
								Log.InfoFormat("Processed audio conversion workflow ({0}) - {1}", saga.CorrelationId, saga.OutPutFilePath);

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
								Log.WarnFormat("Error processing audio conversion workflow ({0}) - {1}", saga.CorrelationId, saga.OutPut);

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
						.Publish((saga, message) => new AudioConversionCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new AudioConversionResponseMessage
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

		public AudioConversionSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual AudioConversionElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual string WorkingDirectoryPath { get; set; }
		public virtual string OutPut { get; set; }
		public virtual string OutPutFilePath { get; set; }

		#region Initialise
		public static Event<AntiVirusRequestMessage> AntiVirusRequestEvent { get; set; }
		#endregion

		#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteAudioConversionWorkflow { get; set; }
		public static Event<IExecutedAudioConversionWorkflowMessage> ExecutedAudioConversionWorkflow { get; set; }
		//public static Event<Fault<IExecuteAudioConversionWorkflowMessage, Guid>> ExecuteAudioConversionWorkflowFailed { get; set; }

		private IExecuteAudioConversionWorkflowMessage GetAudioConversionWorkflowMessage()
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

		private IExecuteAudioConversionWorkflowMessage GetCommandMessage(IAudioSettings audioSetting)
		{
			return new ExecuteAudioConversionWorkflowMessage();
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
