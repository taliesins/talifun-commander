using System;
using System.Collections.Generic;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.CommandLine.Command.Arguments;
using Talifun.Commander.Command.CommandLine.Command.Events;
using Talifun.Commander.Command.CommandLine.Command.Request;
using Talifun.Commander.Command.CommandLine.Command.Response;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.Configuration;
using log4net;

namespace Talifun.Commander.Command.CommandLine.Command
{
	[Serializable]
	public class CommandLineSaga: SagaStateMachine<CommandLineSaga>, ISaga
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(CommandLineSaga));

		static CommandLineSaga()
		{
			Define(() =>
			{
				Initially(
					When(CommandLineRequestEvent)
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
						.Publish((saga, message) => new CommandLineStartedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							Prefix = CommandLineConfiguration.Instance.ConversionType,
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
							var commandMessage = saga.GetCommandLineWorkflowMessage();
							saga.Bus.Publish(commandMessage.GetType(), commandMessage);
						})
						.TransitionTo(WaitingForExecuteCommandLineWorkflow)
				);

				During(
					WaitingForExecuteCommandLineWorkflow,
					When(ExecutedCommandLineWorkflow)
						.Then((saga, message)=>
						{
						    saga.OutPutFilePath = message.OutPutFilePath;
							saga.OutPut = message.Output;

							if (message.EncodeSuccessful)
							{
								if (Log.IsInfoEnabled)
								{
									Log.InfoFormat("Processed command line workflow ({0}) - {1}", saga.CorrelationId, saga.OutPut);
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
									Log.WarnFormat("Error processing command line workflow ({0}) - {1}", saga.CorrelationId, saga.OutPut);
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
						.Publish((saga, message) => new CommandLineCompletedMessage
						{
							CorrelationId = saga.RequestorCorrelationId,
							InputFilePath = saga.InputFilePath,
							FileMatch = saga.FileMatch
						})
						.Publish((saga, message) => new CommandLineResponseMessage
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

		public CommandLineSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual Guid RequestorCorrelationId { get; set; }
		public virtual IDictionary<string, string> AppSettings { get; set; }
		public virtual CommandLineElement Configuration { get; set; }
		public virtual FileMatchElement FileMatch { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual string WorkingDirectoryPath { get; set; }
		public virtual string OutPut { get; set; }
		public virtual string OutPutFilePath { get; set; }

		#region Initialise
		public static Event<CommandLineRequestMessage> CommandLineRequestEvent { get; set; }
		#endregion

		#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
		#endregion

		#region Run command

		public static State WaitingForExecuteCommandLineWorkflow { get; set; }
		public static Event<IExecutedCommandLineWorkflowMessage> ExecutedCommandLineWorkflow { get; set; }
		//public static Event<Fault<IExecuteCommandLineWorkflowMessage, Guid>> ExecuteCommandLineWorkflowFailed { get; set; }

		private IExecuteCommandLineWorkflowMessage GetCommandLineWorkflowMessage()
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

		private ICommandLineParameters GetCommandSettings(CommandLineElement commandLine)
		{
			var args = commandLine.Args
				.Replace("{%Name%}", commandLine.Name)
				.Replace("{%OutPutPath%}", commandLine.GetOutPutPathOrDefault())
				.Replace("{%WorkingPath%}", commandLine.GetWorkingPathOrDefault())
				.Replace("{%ErrorProcessingPath%}", commandLine.GetErrorProcessingPathOrDefault())
				.Replace("{%FileNameFormat%}", commandLine.FileNameFormat)
				.Replace("{%CommandPath%}", commandLine.CommandPath);

			var commandLineSettings = new CommandLineParameters
			{
				CommandArguments = args,
				CommandPath = commandLine.CommandPath
			};
			return commandLineSettings;
		}

		private IExecuteCommandLineWorkflowMessage GetCommandMessage(ICommandLineParameters commandLineParameters)
		{
			return new ExecuteCommandLineWorkflowMessage();
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
