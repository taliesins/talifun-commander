using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.AntiVirus.Command.Events;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.Command.Response;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.AntiVirus.Properties;
using Talifun.Commander.Command.Configuration;
using log4net;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	[Serializable]
	public class AntiVirusSaga : SagaStateMachine<AntiVirusSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(AntiVirusSaga));

		static AntiVirusSaga()
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

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Started ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new AntiVirusStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							Prefix = AntiVirusConfiguration.Instance.ConversionType,
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
						    var commandMessage = saga.GetAntiVirusWorkflowMessage();
                            saga.Bus.Publish(commandMessage, commandMessage.GetType());
						})
						.TransitionTo(WaitingForExecuteAntiVirusWorkflow)
				);

				During(
					WaitingForExecuteAntiVirusWorkflow,
					When(ExecutedAntiVirusWorkflow)
						.Then((saga, message)=>
						{
						    saga.OutPutFilePath = message.OutPutFilePath;

							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Processed antivirus workflow ({0}) - {1}", saga.CorrelationId, saga.OutPutFilePath);
							}
						})
						.Publish((saga, message) => new MoveProcessedFileIntoOutputDirectoryMessage()
						{
							CorrelationId = saga.CorrelationId,
							OutputFilePath = saga.OutPutFilePath,
							OutputDirectoryPath = saga.Configuration.OutPutPath
						})
						.TransitionTo(WaitingForMoveProcessedFileIntoOutputDirectory)
				);

				During(
					WaitingForMoveProcessedFileIntoOutputDirectory,
					When(MovedProcessedFileIntoOutputDirectory)
						.Then((saga, message) =>
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
					WaitingForDeleteTempDirectory,
					When(DeletedTempDirectory)
						.Then((saga, message) =>
						{
							if (Log.IsInfoEnabled)
							{
								Log.InfoFormat("Deleted Temp Directory ({0}) - {1} ", saga.CorrelationId, saga.WorkingDirectoryPath);
								Log.InfoFormat("Completed ({0}) - {1} ", saga.CorrelationId, saga.FileMatch);
							}
						})
						.Publish((saga, message) => new AntiVirusCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new AntiVirusResponseMessage
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

		public AntiVirusSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual AntiVirusElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual string WorkingDirectoryPath { get; set; }
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

		public static State WaitingForExecuteAntiVirusWorkflow { get; set; }
		public static Event<IExecutedAntiVirusWorkflowMessage> ExecutedAntiVirusWorkflow { get; set; }
		//public static Event<Fault<IExecuteAntiVirusWorkflowMessage, Guid>> ExecuteAntiVirusWorkflowFailed { get; set; }

		private IExecuteAntiVirusWorkflowMessage GetAntiVirusWorkflowMessage()
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

		private IAntiVirusSettings GetCommandSettings(AntiVirusElement antiVirusSetting)
		{
			switch (antiVirusSetting.VirusScannerType)
			{
				case VirusScannerType.NotSpecified:
				case VirusScannerType.McAfee:
					return new McAfeeSettings();
				default:
					throw new Exception(Resource.ErrorMessageUnknownVirusScannerType);
			}
		}

		private IExecuteAntiVirusWorkflowMessage GetCommandMessage(IAntiVirusSettings antiVirusSetting)
		{
			return new ExecuteMcAfeeWorkflowMessage();
		}
		#endregion

		#region Move Processed File To Original Directory
		public static State WaitingForMoveProcessedFileIntoOutputDirectory { get; set; }
		public static Event<MovedProcessedFileIntoOutputDirectoryMessage> MovedProcessedFileIntoOutputDirectory { get; set; }
		//public static Event<Fault<MoveProcessedFileIntoCompletedDirectoryMessage, Guid>> MoveProcessedFileIntoOutputDirectoryMessageFailed { get; set; }
		#endregion

		#region Delete Temp Directory
		public static State WaitingForDeleteTempDirectory { get; set; }
		public static Event<DeletedTempDirectoryMessage> DeletedTempDirectory { get; set; }
		//public static Event<Fault<DeleteTempDirectoryMessage, Guid>> DeleteTempDirectoryFailed { get; set; }
		#endregion
	}
}
