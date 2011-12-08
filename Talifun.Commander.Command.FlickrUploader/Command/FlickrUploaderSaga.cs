using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.FlickrUploader.Command.Events;
using Talifun.Commander.Command.FlickrUploader.Command.Request;
using Talifun.Commander.Command.FlickrUploader.Command.Response;
using Talifun.Commander.Command.FlickrUploader.Command.Settings;
using Talifun.Commander.Command.FlickrUploader.Configuration;
using log4net;

namespace Talifun.Commander.Command.FlickrUploader.Command
{
	[Serializable]
	public class FlickrUploaderSaga: SagaStateMachine<FlickrUploaderSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(FlickrUploaderSaga));

		static FlickrUploaderSaga()
		{
			Define(() =>
			{
				Initially(
					When(FlickrUploaderRequestEvent)
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
						.Publish((saga, message) => new FlickrUploaderStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new RetrieveMetaDataMessage
						{
							CorrelationId = saga.CorrelationId,
							InputFilePath = saga.InputFilePath
						})
						.TransitionTo(WaitingForRetrieveMetaData)
					);

				During(
					WaitingForRetrieveMetaData,
					When(RetrievedMetaData)
						.Then((saga, message) =>
						{
							saga.MetaData = message.MetaData;

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Retrieved Meta Data ({0})", saga.CorrelationId);
							}
						})
						.Then((saga, message) =>
						{
							var commandMessage = saga.GetFlickrUploaderWorkflowMessage();
							saga.Bus.Publish(commandMessage.GetType(), commandMessage);
						})
						.TransitionTo(WaitingForExecuteFlickrUploaderWorkflow)
				);

				During(
					WaitingForExecuteFlickrUploaderWorkflow,
					When(ExecutedFlickrUploaderWorkflow).RetryLater()
						.Then((saga, message) =>
						{
							if (message.Cancelled)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Cancelled Flickr uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else if (message.Error == null)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Processed Flickr uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else
							{
								if (Log.IsInfoEnabled)
								{
									Log.WarnFormat("Error processing Flickr uploader workflow ({0})", saga.CorrelationId);
								}
							}

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Completed ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new FlickrUploaderCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new FlickrUploaderResponseMessage
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

		public FlickrUploaderSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual FlickrUploaderElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual FlickrMetaData MetaData { get; set; }

		#region Initialise
		public static Event<FlickrUploaderRequestMessage> FlickrUploaderRequestEvent { get; set; }
		#endregion

		#region Retrieve Meta Data
		public static State WaitingForRetrieveMetaData { get; set; }
		public static Event<RetrievedMetaDataMessage> RetrievedMetaData { get; set; }
		//public static Event<Fault<RetrieveMetaDataMessage, Guid>> RetrieveMetaDataFailed { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteFlickrUploaderWorkflow { get; set; }
		public static Event<IExecutedFlickrUploaderWorkflowMessage> ExecutedFlickrUploaderWorkflow { get; set; }
		//public static Event<Fault<IExecuteFlickrUploaderWorkflowMessage, Guid>> ExecuteFlickrUploaderWorkflowFailed { get; set; }

		private IExecuteFlickrUploaderWorkflowMessage GetFlickrUploaderWorkflowMessage()
		{
			var commandSettings = GetCommandSettings(Configuration);
			commandSettings.MetaData = MetaData;

			var commandMessage = GetCommandMessage(commandSettings);

			commandMessage.CorrelationId = CorrelationId;
			commandMessage.AppSettings = AppSettings;
			commandMessage.InputFilePath = InputFilePath;
			commandMessage.Settings = commandSettings;
			return commandMessage;
		}

		private IFlickrUploaderSettings GetCommandSettings(FlickrUploaderElement flickrUploader)
		{
			return new FlickrUploaderSettings()
			{
				Authentication = new AuthenticationSettings()
				{
					FlickrApiKey = flickrUploader.FlickrApiKey,
					FlickrApiSecret = flickrUploader.FlickrApiSecret,
					FlickrFrob = flickrUploader.FlickrFrob,
					FlickrAuthToken = flickrUploader.FlickrAuthToken,
				}
			};
		}

		private IExecuteFlickrUploaderWorkflowMessage GetCommandMessage(IFlickrUploaderSettings flickrUploaderSettings)
		{
			return new ExecuteFlickrUploaderWorkflowMessage();
		}
		#endregion
	}
}
