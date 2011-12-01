using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.YouTubeUploader.Command.Events;
using Talifun.Commander.Command.YouTubeUploader.Command.Request;
using Talifun.Commander.Command.YouTubeUploader.Command.Response;
using Talifun.Commander.Command.YouTubeUploader.Command.Settings;
using Talifun.Commander.Command.YouTubeUploader.Configuration;
using log4net;

namespace Talifun.Commander.Command.YouTubeUploader.Command
{
	[Serializable]
	public class YouTubeUploaderSaga: SagaStateMachine<YouTubeUploaderSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(YouTubeUploaderSaga));

		static YouTubeUploaderSaga()
		{
			Define(() =>
			{
				Initially(
					When(YouTubeUploaderRequestEvent)
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
						.Publish((saga, message) => new YouTubeUploaderStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Then((saga, message) =>
						{
							var commandMessage = saga.GetYouTubeUploaderWorkflowMessage();
							saga.Bus.Publish(commandMessage.GetType(), commandMessage);
						})
						.TransitionTo(WaitingForExecuteAudioConversionWorkflow)
					);

				During(
					WaitingForExecuteAudioConversionWorkflow,
					When(ExecutedYouTubeUploaderWorkflow).RetryLater()
						.Then((saga, message) =>
						{
							if (message.Cancelled)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Cancelled YouTube uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else if (message.Error == null)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Processed YouTube uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else
							{
								if (Log.IsInfoEnabled)
								{
									Log.WarnFormat("Error processing YouTube uploader workflow ({0})", saga.CorrelationId);
								}
							}

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Completed ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new YouTubeUploaderCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new YouTubeUploaderResponseMessage
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

		public YouTubeUploaderSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual YouTubeUploaderElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }

		#region Initialise
		public static Event<YouTubeUploaderRequestMessage> YouTubeUploaderRequestEvent { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteAudioConversionWorkflow { get; set; }
		public static Event<IExecutedYouTubeUploaderWorkflowMessage> ExecutedYouTubeUploaderWorkflow { get; set; }
		//public static Event<Fault<IExecuteYouTubeUploaderWorkflowMessage, Guid>> ExecuteYouTubeUploaderWorkflowFailed { get; set; }

		private IExecuteYouTubeUploaderWorkflowMessage GetYouTubeUploaderWorkflowMessage()
		{
			var commandSettings = GetCommandSettings(Configuration);
			var commandMessage = GetCommandMessage(commandSettings);

			commandMessage.CorrelationId = CorrelationId;
			commandMessage.AppSettings = AppSettings;
			commandMessage.InputFilePath = InputFilePath;
			commandMessage.Settings = commandSettings;
			return commandMessage;
		}

		private IYouTubeUploaderSettings GetCommandSettings(YouTubeUploaderElement youTubeUploader)
		{
			return new YouTubeUploaderSettings()
			{
				Authentication = new AuthenticationSettings()
				{
					GooglePassword = youTubeUploader.GooglePassword,
					GoogleUsername = youTubeUploader.GoogleUsername,
					YouTubeUsername = youTubeUploader.YouTubeUsername
				},
				Upload = new UploadSettings()
				{
					ChunkSize = 25
				}
			};
		}

		private IExecuteYouTubeUploaderWorkflowMessage GetCommandMessage(IYouTubeUploaderSettings youTubeUploaderSettings)
		{
			return new ExecuteYouTubeUploaderWorkflowMessage();
		}
		#endregion
	}
}
