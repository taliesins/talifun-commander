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

namespace Talifun.Commander.Command.AntiVirus.Command
{
	[Serializable]
	public class AntiVirusSaga : SagaStateMachine<AntiVirusSaga>, ISaga
	{
		static AntiVirusSaga()
		{
			Define(() =>
			{
				Initially(
					When(AntiVirusRequestEvent)
						.Then((saga, message) =>
						{
							saga.AppSettings = message.AppSettings;
							saga.Configuration = message.Configuration;
							saga.FileMatch = message.FileMatch;
							saga.InputFilePath = message.InputFilePath;
						})
						.Publish((saga, message) => new AntiVirusStartedMessage
						{
							CorrelationId = saga.CorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.TransitionTo(WaitingForCreateTempDirectory)
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							Prefix = AntiVirusConfiguration.Instance.ConversionType,
							InputFilePath = saga.InputFilePath,
							WorkingDirectoryPath = saga.Configuration.GetWorkingPathOrDefault()
						})
					);

				During(
					WaitingForCreateTempDirectory,
					When(CreatedTempDirectory)
						.Then((saga, message) =>
						{
							saga.WorkingDirectoryPath = message.WorkingDirectoryPath;
						})
						.TransitionTo(WaitingForExecuteAntiVirusWorkflow)
						.Then((saga, message)=>
						{
						    var commandMessage = saga.GetAntiVirusWorkflowMessage();
							saga.Bus.Publish(commandMessage.GetType(), commandMessage);
						})
				);

				During(
					WaitingForExecuteAntiVirusWorkflow,
					When(ExecutedAntiVirusWorkflow)
						.Then((saga, message)=>
						{
						    saga.OutPutFilePath = message.OutPutFilePath;
						})
						.TransitionTo(WaitingForMoveProcessedFileIntoOutputDirectory)
						.Publish((saga, message) => new MoveProcessedFileIntoOutputDirectoryMessage()
						{
							CorrelationId = saga.CorrelationId,
							OutputPath = saga.OutPutFilePath,
							WorkingDirectoryPath = saga.WorkingDirectoryPath
						})
				);

				During(
					WaitingForMoveProcessedFileIntoOutputDirectory,
					When(MovedProcessedFileIntoOutputDirectory)
						.TransitionTo(WaitingForDeleteTempDirectory)
						.Publish((saga, message) => new DeleteTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							WorkingPath = saga.WorkingDirectoryPath
						})
				);

				During(
					WaitingForDeleteTempDirectory,
					When(DeletedTempDirectory)
						.Publish((saga, message) => new AntiVirusResponseMessage
						{
							CorrelationId = saga.CorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new AntiVirusCompletedMessage
						{
							CorrelationId = saga.CorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
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

			commandMessage.AppSettings = AppSettings;
			commandMessage.InputFilePath = InputFilePath;
			commandMessage.OutputDirectoryPath = OutPutFilePath;
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
