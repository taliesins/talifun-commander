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
						.Publish((saga, message) => new AntiVirusStartedMessage
						{
							CorrelationId = message.CorrelationId,
							InputFilePath = message.InputFilePath,
							FileMatch = message.FileMatch
						})
						.Then((saga, message) =>
						{
							saga.AppSettings = message.AppSettings;
							saga.Configuration = message.Configuration;
							saga.FileMatch = message.FileMatch;
							saga.InputFilePath = message.InputFilePath;
						})
						.TransitionTo(WaitingForCreateTempDirectory)
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = message.CorrelationId,
							InputFilePath = saga.InputFilePath
						})
					);

				During(
					WaitingForCreateTempDirectory,
					When(CreatedTempDirectory)
						.Then((saga, message) =>
						{
							saga.WorkingPath = message.WorkingPath;
						})
						.TransitionTo(WaitingForCommandToExecute)
						.Then((saga, message)=>
						{
						    var commandMessage = saga.GetCommandMessage();
							saga.Bus.Publish(message.GetType(), commandMessage);
						})
				);

				During(
					WaitingForMoveProcessedFileIntoOutputDirectory,
					When(MovedProcessedFileIntoOutputDirectory)
						.TransitionTo(WaitingForDeleteTempDirectory)
						.Publish((saga, message) => new DeleteTempDirectoryMessage
						{
							CorrelationId = message.CorrelationId,
							WorkingPath = saga.WorkingPath
						})
				);

				During(
					WaitingForDeleteTempDirectory,
					When(DeletedTempDirectory)
						.Publish((saga, message) => new AntiVirusResponseMessage
						{
							CorrelationId = message.CorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new AntiVirusCompletedMessage
						{
							CorrelationId = message.CorrelationId,
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
		public virtual string WorkingPath { get; set; }

		#region Initialise
		public static Event<AntiVirusRequestMessage> AntiVirusRequestEvent { get; set; }
		#endregion

		#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
		#endregion

		#region Run command

		public static State WaitingForCommandToExecute { get; set; }

		private object GetCommandMessage()
		{
			var commandSettings = GetCommandSettings(Configuration);

			return null;
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

		private ICommand<IAntiVirusSettings> GetCommand(IAntiVirusSettings antiVirusSetting)
		{
			return new McAfeeCommand();
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
