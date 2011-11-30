using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Image.Command.Events;
using Talifun.Commander.Command.Image.Command.ImageSettings;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.Command.Response;
using Talifun.Commander.Command.Image.Configuration;
using log4net;

namespace Talifun.Commander.Command.Image.Command
{
	[Serializable]
	public class ImageConversionSaga: SagaStateMachine<ImageConversionSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ImageConversionSaga));

		static ImageConversionSaga()
		{
			Define(() =>
			{
				Initially(
					When(ImageConversionRequestEvent)
						.Then((saga, message) =>
						{
							saga.RequestorCorrelationId = message.RequestorCorrelationId;
							saga.AppSettings = message.AppSettings;
							saga.Configuration = message.Configuration;
							saga.FileMatch = message.FileMatch;
							saga.InputFilePath = message.InputFilePath;

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Started ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new ImageConversionStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							Prefix = ImageConversionConfiguration.Instance.ConversionType,
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

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Created Temp Directory ({0}) - {1}", saga.CorrelationId, saga.WorkingDirectoryPath);
							}
						})
						.Then((saga, message)=>
						{
						    var commandMessage = saga.GetImageConversionWorkflowMessage();
							saga.Bus.Publish(commandMessage.GetType(), commandMessage);
						})
						.TransitionTo(WaitingForExecuteImageConversionWorkflow)
				);

				During(
					WaitingForExecuteImageConversionWorkflow,
					When(ExecutedImageConversionWorkflow)
						.Then((saga, message)=>
						{
						    saga.OutPutFilePath = message.OutPutFilePath;
							saga.OutPut = message.Output;

							if (message.EncodeSuccessful)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Processed audio conversion workflow ({0}) - {1}", saga.CorrelationId, saga.OutPut);
								}

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
								if (Log.IsInfoEnabled)
								{
									Log.WarnFormat("Error processing audio conversion workflow ({0}) - {1}", saga.CorrelationId, saga.OutPut);
								}

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
							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Moved processed file to output directory  ({0}) - {1}", saga.CorrelationId,
								               message.OutputFilePath);
							}
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
							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Moved errored file to error directory ({0}) - {1}", saga.CorrelationId, message.OutputFilePath);
							}
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
							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Deleted Temp Directory ({0}) - {1} ", saga.CorrelationId, saga.WorkingDirectoryPath);
								Log.InfoFormat("Completed ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new ImageConversionCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new ImageConversionResponseMessage
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

		public ImageConversionSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual ImageConversionElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual string WorkingDirectoryPath { get; set; }
		public virtual string OutPut { get; set; }
		public virtual string OutPutFilePath { get; set; }

		#region Initialise
		public static Event<ImageConversionRequestMessage> ImageConversionRequestEvent { get; set; }
		#endregion

		#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteImageConversionWorkflow { get; set; }
		public static Event<IExecutedImageConversionWorkflowMessage> ExecutedImageConversionWorkflow { get; set; }
		//public static Event<Fault<IExecuteImageConversionWorkflowMessage, Guid>> ExecuteImageConversionWorkflowFailed { get; set; }

		private IExecuteImageConversionWorkflowMessage GetImageConversionWorkflowMessage()
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

		private IImageResizeSettings GetCommandSettings(ImageConversionElement imageConversion)
		{
			var imageResizeSettings = new ImageResizeSettings
			{
				BackgroundColour = imageConversion.BackgroundColor,
				Gravity = imageConversion.Gravity,
				ResizeMode = imageConversion.ResizeMode,
				ResizeImageType = imageConversion.ResizeImageType,
				Quality = imageConversion.Quality,
				WatermarkPath = imageConversion.WatermarkPath,
				WatermarkDissolveLevels = imageConversion.WatermarkDissolveLevels,
				WatermarkGravity = imageConversion.WatermarkGravity,
			};

			if (imageConversion.Height != 0)
			{
				imageResizeSettings.Height = imageConversion.Height;
			}

			if (imageConversion.Width != 0)
			{
				imageResizeSettings.Width = imageConversion.Width;
			}

			return imageResizeSettings;
		}

		private IExecuteImageConversionWorkflowMessage GetCommandMessage(IImageResizeSettings commandLineSettings)
		{
			return new ExecuteImageConversionWorkflowMessage();
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
