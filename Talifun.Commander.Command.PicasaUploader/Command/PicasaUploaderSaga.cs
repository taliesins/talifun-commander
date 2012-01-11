using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.PicasaUploader.Command.Events;
using Talifun.Commander.Command.PicasaUploader.Command.Request;
using Talifun.Commander.Command.PicasaUploader.Command.Response;
using Talifun.Commander.Command.PicasaUploader.Command.Settings;
using Talifun.Commander.Command.PicasaUploader.Configuration;
using log4net;

namespace Talifun.Commander.Command.PicasaUploader.Command
{
	[Serializable]
	public class PicasaUploaderSaga: SagaStateMachine<PicasaUploaderSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(PicasaUploaderSaga));

		static PicasaUploaderSaga()
		{
			Define(() =>
			{
				Initially(
					When(PicasaUploaderRequestEvent)
						.Then((saga, message) =>
						{
							saga.RequestorCorrelationId = message.RequestorCorrelationId;
							saga.AppSettings = message.AppSettings;
							saga.Configuration = message.Configuration;
							saga.FileMatch = message.FileMatch;
							saga.InputFilePath = message.InputFilePath;

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Started ({0}/{1}) - {2} ", saga.RequestorCorrelationId, saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new PicasaUploaderStartedMessage
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
							var commandMessage = saga.GetPicasaUploaderWorkflowMessage();
							saga.Bus.Publish(commandMessage.GetType(), commandMessage);
						})
						.TransitionTo(WaitingForExecutePicasaUploaderWorkflow)
				);

				During(
					WaitingForExecutePicasaUploaderWorkflow,
					When(ExecutedPicasaUploaderWorkflow).RetryLater()
						.Then((saga, message) =>
						{
							if (message.Cancelled)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Cancelled Picasa uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else if (message.Error == null)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Processed Picasa uploader workflow ({0})", saga.CorrelationId);
								}
							}
							else
							{
								if (Log.IsInfoEnabled)
								{
									Log.WarnFormat("Error processing Picasa uploader workflow ({0})", saga.CorrelationId);
								}
							}

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Completed ({0}/{1}) - {2} ", saga.RequestorCorrelationId, saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new PicasaUploaderCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new PicasaUploaderResponseMessage
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

		public PicasaUploaderSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual PicasaUploaderElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual PicasaMetaData MetaData { get; set; }

		#region Initialise
		public static Event<PicasaUploaderRequestMessage> PicasaUploaderRequestEvent { get; set; }
		#endregion

		#region Retrieve Meta Data
		public static State WaitingForRetrieveMetaData { get; set; }
		public static Event<RetrievedMetaDataMessage> RetrievedMetaData { get; set; }
		//public static Event<Fault<RetrieveMetaDataMessage, Guid>> RetrieveMetaDataFailed { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecutePicasaUploaderWorkflow { get; set; }
		public static Event<IExecutedPicasaUploaderWorkflowMessage> ExecutedPicasaUploaderWorkflow { get; set; }
		//public static Event<Fault<IExecutePicasaUploaderWorkflowMessage, Guid>> ExecutePicasaUploaderWorkflowFailed { get; set; }

		private IExecutePicasaUploaderWorkflowMessage GetPicasaUploaderWorkflowMessage()
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

		private IPicasaUploaderSettings GetCommandSettings(PicasaUploaderElement picasaUploader)
		{
			return new PicasaUploaderSettings()
			{
				Authentication = new AuthenticationSettings()
				{
					GooglePassword = picasaUploader.GooglePassword,
					GoogleUsername = picasaUploader.GoogleUsername,
					PicasaUsername = picasaUploader.PicasaUsername,
					ApplicationName = picasaUploader.ApplicationName,
					AlbumId = picasaUploader.AlbumId,
				},
				Upload = new UploadSettings()
				{
					ChunkSize = 25
				}
			};
		}

		private IExecutePicasaUploaderWorkflowMessage GetCommandMessage(IPicasaUploaderSettings picasaUploaderSettings)
		{
			return new ExecutePicasaUploaderWorkflowMessage();
		}
		#endregion
	}
}
