using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.BoxNetUploader.Command.Events;
using Talifun.Commander.Command.BoxNetUploader.Command.Request;
using Talifun.Commander.Command.BoxNetUploader.Command.Response;
using Talifun.Commander.Command.BoxNetUploader.Command.Settings;
using Talifun.Commander.Command.BoxNetUploader.Configuration;
using log4net;

namespace Talifun.Commander.Command.BoxNetUploader.Command
{
	[Serializable]
	public class BoxNetUploaderSaga: SagaStateMachine<BoxNetUploaderSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(BoxNetUploaderSaga));

		static BoxNetUploaderSaga()
		{
			Define(() =>
			{
				Initially(
					When(BoxNetUploaderRequestEvent)
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
						.Publish((saga, message) => new BoxNetUploaderStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Then((saga, message) =>
						{
							var commandMessage = saga.GetBoxNetUploaderWorkflowMessage();
                            saga.Bus.Publish(commandMessage, commandMessage.GetType());
						})
						.TransitionTo(WaitingForExecuteBoxNetUploaderWorkflow)
					);

				During(
					WaitingForExecuteBoxNetUploaderWorkflow,
					When(ExecutedBoxNetUploaderWorkflow).RetryLater()
						.Then((saga, message) =>
						{
							if (message.Cancelled)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Cancelled BoxNet uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else if (message.Error == null)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Processed BoxNet uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else
							{
								if (Log.IsInfoEnabled)
								{
									Log.WarnFormat("Error processing BoxNet uploader workflow ({0})", saga.CorrelationId);
								}
							}

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Completed ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new BoxNetUploaderCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new BoxNetUploaderResponseMessage
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

		public BoxNetUploaderSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual BoxNetUploaderElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }

		#region Initialise
		public static Event<BoxNetUploaderRequestMessage> BoxNetUploaderRequestEvent { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteBoxNetUploaderWorkflow { get; set; }
		public static Event<IExecutedBoxNetUploaderWorkflowMessage> ExecutedBoxNetUploaderWorkflow { get; set; }
		//public static Event<Fault<IExecuteBoxNetUploaderWorkflowMessage, Guid>> ExecuteBoxNetUploaderWorkflowFailed { get; set; }

		private IExecuteBoxNetUploaderWorkflowMessage GetBoxNetUploaderWorkflowMessage()
		{
			var commandSettings = GetCommandSettings(Configuration);

			var commandMessage = GetCommandMessage(commandSettings);

			commandMessage.CorrelationId = CorrelationId;
			commandMessage.AppSettings = AppSettings;
			commandMessage.InputFilePath = InputFilePath;
			commandMessage.Settings = commandSettings;
			return commandMessage;
		}

		private IBoxNetUploaderSettings GetCommandSettings(BoxNetUploaderElement flickrUploader)
		{
			return new BoxNetUploaderSettings
			{
				Authentication = new AuthenticationSettings
				{
					BoxNetUsername = flickrUploader.BoxNetUsername,
					BoxNetPassword = flickrUploader.BoxNetPassword
				},
				Folder = flickrUploader.BoxNetFolder
			};
		}

        private IExecuteBoxNetUploaderWorkflowMessage GetCommandMessage(IBoxNetUploaderSettings boxNetUploaderSettings)
		{
			return new ExecuteBoxNetUploaderWorkflowMessage();
		}
		#endregion
	}
}
