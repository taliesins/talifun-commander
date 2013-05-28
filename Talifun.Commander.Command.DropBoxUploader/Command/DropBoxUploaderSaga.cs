using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.DropBoxUploader.Command.Events;
using Talifun.Commander.Command.DropBoxUploader.Command.Request;
using Talifun.Commander.Command.DropBoxUploader.Command.Response;
using Talifun.Commander.Command.DropBoxUploader.Command.Settings;
using Talifun.Commander.Command.DropBoxUploader.Configuration;
using log4net;

namespace Talifun.Commander.Command.DropBoxUploader.Command
{
	[Serializable]
	public class DropBoxUploaderSaga: SagaStateMachine<DropBoxUploaderSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(DropBoxUploaderSaga));

		static DropBoxUploaderSaga()
		{
			Define(() =>
			{
				Initially(
					When(DropBoxUploaderRequestEvent)
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
						.Publish((saga, message) => new DropBoxUploaderStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Then((saga, message) =>
						{
							var commandMessage = saga.GetDropBoxUploaderWorkflowMessage();
                            saga.Bus.Publish(commandMessage, commandMessage.GetType());
						})
						.TransitionTo(WaitingForExecuteDropBoxUploaderWorkflow)
					);

				During(
					WaitingForExecuteDropBoxUploaderWorkflow,
					When(ExecutedDropBoxUploaderWorkflow).RetryLater()
						.Then((saga, message) =>
						{
							if (message.Cancelled)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Cancelled DropBox uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else if (message.Error == null)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Processed DropBox uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else
							{
								if (Log.IsInfoEnabled)
								{
									Log.WarnFormat("Error processing DropBox uploader workflow ({0})", saga.CorrelationId);
								}
							}

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Completed ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new DropBoxUploaderCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new DropBoxUploaderResponseMessage
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

		public DropBoxUploaderSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual DropBoxUploaderElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }

		#region Initialise
		public static Event<DropBoxUploaderRequestMessage> DropBoxUploaderRequestEvent { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteDropBoxUploaderWorkflow { get; set; }
		public static Event<IExecutedDropBoxUploaderWorkflowMessage> ExecutedDropBoxUploaderWorkflow { get; set; }
		//public static Event<Fault<IExecuteDropBoxUploaderWorkflowMessage, Guid>> ExecuteDropBoxUploaderWorkflowFailed { get; set; }

		private IExecuteDropBoxUploaderWorkflowMessage GetDropBoxUploaderWorkflowMessage()
		{
			var commandSettings = GetCommandSettings(Configuration);

			var commandMessage = GetCommandMessage(commandSettings);

			commandMessage.CorrelationId = CorrelationId;
			commandMessage.AppSettings = AppSettings;
			commandMessage.InputFilePath = InputFilePath;
			commandMessage.Settings = commandSettings;
			return commandMessage;
		}

		private IDropBoxUploaderSettings GetCommandSettings(DropBoxUploaderElement flickrUploader)
		{
			return new DropBoxUploaderSettings
			{
				Authentication = new AuthenticationSettings
				{
					DropBoxApiKey = flickrUploader.DropBoxApiKey,
					DropBoxApiSecret = flickrUploader.DropBoxApiSecret,
					DropBoxRequestKey = flickrUploader.DropBoxRequestKey,
					DropBoxRequestSecret = flickrUploader.DropBoxRequestSecret,
					DropBoxAuthenticationKey = flickrUploader.DropBoxAuthenticationKey,
					DropBoxAuthenticationSecret = flickrUploader.DropBoxAuthenticationSecret,
				},
				Folder = flickrUploader.DropBoxFolder
			};
		}

		private IExecuteDropBoxUploaderWorkflowMessage GetCommandMessage(IDropBoxUploaderSettings dropBoxUploaderSettings)
		{
			return new ExecuteDropBoxUploaderWorkflowMessage();
		}
		#endregion
	}
}
