using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Magnum;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FileMatcher.Request;
using Talifun.Commander.Command.FileMatcher.Response;
using Talifun.Commander.Command.FolderWatcher.Messages;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.FileMatcher
{
	[Serializable]
	public class FileMatcherSaga : SagaStateMachine<FileMatcherSaga>, ISaga
	{
		static FileMatcherSaga()
		{
			Define(() =>
			{
				Initially(
					When(FileFinishedChangingEvent)
						.Then((saga, message)=>
						{
							saga.InputFilePath = message.FilePath;
							saga.Folder = message.Folder;
						})
						.Publish((saga, message) => new CreateTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							InputFilePath = saga.InputFilePath,
							WorkingPath = saga.Folder.GetWorkingPathOrDefault()
						})
						.TransitionTo(WaitingForCreateTempDirectory)
					);

				During(
					WaitingForCreateTempDirectory,
					When(CreatedTempDirectory)
						.Then((saga, message) =>
						{
						    saga.WorkingFilePath = message.WorkingFilePath;
						})
						.Publish((saga, message) => new MoveFileToBeProcessedIntoTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							FilePath = saga.InputFilePath,
							WorkingFilePath = saga.WorkingFilePath
						})
						.TransitionTo(WaitingForMoveFileToBeProcessedIntoTempDirectory)
				);

				During(
					WaitingForMoveFileToBeProcessedIntoTempDirectory,
					When(MovedFileToBeProcessedIntoTempDirectory)
					    .Then((saga, message) =>
					    {
							saga.FileMatchesToExecute = saga.GetFileMatchesToExecute();      		
					    })
						.Publish((saga, message) => new ProcessFileMatchesMessage
						{
							CorrelationId = saga.CorrelationId,
							WorkingFilePath = saga.WorkingFilePath,
							FileMatches = saga.FileMatchesToExecute
						})
						.TransitionTo(WaitingForPluginsToProcess)
				);

				During(
					WaitingForPluginsToProcess,
					When(StartPluginsExecution)
						.Then((saga, message) =>
						{
						    saga.RaiseEvent(ExecuteNextPlugin, new ExecuteNextPluginMessage
						    {
						      	CorrelationId = saga.CorrelationId
						    });
						}),
					When(ExecuteNextPlugin)
						.Then((saga, message) =>
						{
						    saga.ExecutePlugin(message);
						}),
					When(PluginResponse)
						.Then((saga, message) =>
						{
						    var fileMatch = saga.FileMatchesToExecute.Where(x => x.CorrelationId == message.ResponderCorrelationId).First();
						    fileMatch.Executed = true;

						    saga.RaiseEvent(ExecuteNextPlugin, new ExecuteNextPluginMessage
						    {
						      	CorrelationId = saga.CorrelationId
						    });
						}),
					When(CompletedPluginsExecution)
						.Publish((saga, message) => new MoveProcessedFileIntoCompletedDirectoryMessage
						{
						    CorrelationId = saga.CorrelationId,
						    WorkingFilePath = saga.WorkingFilePath,
						    CompletedPath = saga.Folder.GetCompletedPathOrDefault()
						})
						.TransitionTo(WaitingForMoveProcessedFileIntoCompletedDirectory)
				);

				During(
					WaitingForMoveProcessedFileIntoCompletedDirectory,
					When(MovedProcessedFileIntoCompletedDirectory)
						.Publish((saga, message) => new DeleteTempDirectoryMessage
						{
							CorrelationId = saga.CorrelationId,
							WorkingFilePath = saga.WorkingFilePath
						})
						.TransitionTo(WaitingForDeleteTempDirectory)
				);

				During(
					WaitingForDeleteTempDirectory,
					When(DeletedTempDirectory)
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

		public FileMatcherSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual string InputFilePath { get; set; }
		public virtual FolderElement Folder { get; set; }
		public virtual string WorkingFilePath { get; set; }
		public virtual IList<SerialWorkflowStep<FileMatchElement>> FileMatchesToExecute { get; set; }
		public virtual bool StopProcessingFileMatches { get; set; }

#region Initialise
		public static Event<FileFinishedChangingMessage> FileFinishedChangingEvent { get; set; }
#endregion

#region Create Temp Directory
		public static State WaitingForCreateTempDirectory { get; set; }
		public static Event<CreatedTempDirectoryMessage> CreatedTempDirectory { get; set; }
		//public static Event<Fault<CreateTempDirectoryMessage, Guid>> CreateTempDirectoryFailed { get; set; }
#endregion

#region Move file to be processed into temp directory
		public static State WaitingForMoveFileToBeProcessedIntoTempDirectory { get; set; }
		public static Event<MovedFileToBeProcessedIntoTempDirectoryMessage> MovedFileToBeProcessedIntoTempDirectory { get; set; }
		//public static Event<Fault<MoveFileToBeProcessedIntoTempDirectoryMessage, Guid>> MoveFileToBeProcessedIntoTempDirectoryFailed { get; set; }

		private IList<SerialWorkflowStep<FileMatchElement>> GetFileMatchesToExecute()
		{
			var fileMatchesToExecute = new List<SerialWorkflowStep<FileMatchElement>>();
			const RegexOptions regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;

			var workingFilePath = new FileInfo(WorkingFilePath);
			var order = 1;
			foreach (var fileMatch in Folder.FileMatches)
			{
				if (string.IsNullOrEmpty(fileMatch.Expression) || Regex.IsMatch(workingFilePath.Name, fileMatch.Expression, regxOptions))
				{
					var value = new SerialWorkflowStep<FileMatchElement>()
					            	{
					            		CorrelationId = CombGuid.Generate(),
					            		MessageInput = fileMatch,
										Executed = false,
										Order = order++
					            	};

					fileMatchesToExecute.Add(value);
				}
			}

			return fileMatchesToExecute;
		}
#endregion

#region Process file matches

		public static State WaitingForPluginsToProcess { get; set; }
		public static Event<ProcessFileMatchesMessage> StartPluginsExecution { get; set; }
		public static Event<ExecuteNextPluginMessage> ExecuteNextPlugin { get; set; }
		public static Event<IPluginResponseMessage> PluginResponse { get; set; }
		//public static Event<Fault<IPluginResponseMessage, Guid>> PluginResponseFailed { get; set; }
		public static Event<ProcessedFileMatchesMessage> CompletedPluginsExecution { get; set; }
		
		private void ExecutePlugin(ExecuteNextPluginMessage message)
		{
			var workingFilePath = new FileInfo(WorkingFilePath);

			var fileMatchToExecute = FileMatchesToExecute
				.Where(x=>!x.Executed)
				.OrderBy(x=>x.Order)
				.FirstOrDefault();

			//If the file no longer exists, it assumed that there should be no more processing
			//e.g. anti-virus may delete file so, we will do no more processing
			//e.g. video process was unable to process file so it was moved to error processing folder
			if (fileMatchToExecute == null || StopProcessingFileMatches || !workingFilePath.Exists)
			{
				//Transition to ProcessedFileMatches
				var processedFileMatchesMessage = new ProcessedFileMatchesMessage
				{
					CorrelationId = CorrelationId
				};

				RaiseEvent(CompletedPluginsExecution, processedFileMatchesMessage);
				return;
			}

			var fileMatch = fileMatchToExecute.MessageInput;
			var fileMatchCorrelationId = fileMatchToExecute.CorrelationId;

			if (fileMatch.StopProcessing)
			{
				StopProcessingFileMatches = true;
			}

			var project = GetCurrentProject(fileMatch);

			var commandConfigurationTester = GetCommandMessenger(fileMatch.ConversionType);
			var pluginRequestMessage = commandConfigurationTester.CreateRequestMessage(fileMatchCorrelationId, CorrelationId, AppSettings.Settings.ToDictionary(), project, WorkingFilePath, fileMatch);

			Bus.Publish(pluginRequestMessage.GetType(), pluginRequestMessage);
		}

		private ICommandMessenger GetCommandMessenger(string conversionType)
		{
			var commandConfigurationTesters = Container.GetExportedValues<ICommandMessenger>();
			var commandConfigurationTester = commandConfigurationTesters
				.Where(x => x.Settings.ConversionType == conversionType)
				.First();

			return commandConfigurationTester;
		}

		private CommanderSection CommanderSettings
		{
			get { return CurrentConfiguration.CommanderSettings; }
		}

		private AppSettingsSection AppSettings
		{
			get { return CurrentConfiguration.AppSettings; }
		}

		private ExportProvider Container
		{
			get { return CurrentConfiguration.Container; }
		}

		private ProjectElement GetCurrentProject(FileMatchElement fileMatch)
		{
			var projects = CommanderSettings.Projects;

			for (var i = 0; i < projects.Count; i++)
			{
				var folders = projects[i].Folders;

				for (var j = 0; j < folders.Count; j++)
				{
					var fileMatches = folders[j].FileMatches;

					for (var k = 0; k < fileMatches.Count; k++)
					{
						if (fileMatches[k].Name == fileMatch.Name) return projects[i];
					}
				}
			}

			throw new Exception(string.Format(Resource.ErrorMessageCannotFindProjectForFileElement));
		}

#endregion

#region Move Processed File To Original Directory
		public static State WaitingForMoveProcessedFileIntoCompletedDirectory { get; set; }
		public static Event<MovedProcessedFileIntoCompletedDirectoryMessage> MovedProcessedFileIntoCompletedDirectory { get; set; }
		//public static Event<Fault<MoveProcessedFileIntoCompletedDirectoryMessage, Guid>> MoveProcessedFileIntoCompletedDirectoryMessageFailed { get; set; }
#endregion

#region Delete Temp Directory
		public static State WaitingForDeleteTempDirectory { get; set; }
		public static Event<DeletedTempDirectoryMessage> DeletedTempDirectory { get; set; }
		//public static Event<Fault<DeleteTempDirectoryMessage, Guid>> DeleteTempDirectoryFailed { get; set; }
#endregion
	}
}
