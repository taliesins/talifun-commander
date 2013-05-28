using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Magnum;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker.Request;
using Talifun.Commander.Command.ConfigurationChecker.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.ConfigurationChecker
{
    [Serializable]
    public class ProjectConfigurationCheckerSaga : SagaStateMachine<ProjectConfigurationCheckerSaga>, ISaga
    {
        static ProjectConfigurationCheckerSaga()
		{
			Define(() =>
			{
				Initially(
                    When(StartTestProjectConfiguration)
						.Then((saga, message) =>
						{
                            saga.RequestorCorrelationId = message.RequestorCorrelationId;
                            saga.Project = message.Project;
						    saga.AppSettings = message.AppSettings;
                            saga.Exceptions = new List<Exception>();							
						})
						.Then((saga, message) => saga.TestCommandConfigurations())
                        .TransitionTo(WaitingForTestedCommandsConfiguration)
				);

				During(
                    WaitingForTestedCommandsConfiguration,
                    When(TestedCommandConfiguration)
						.Then((saga, message) =>
						{
							var commandConfigurationToTest = saga.CommandConfigurationsToTest.Where(x => x.CorrelationId == message.ResponderCorrelationId).First();
							commandConfigurationToTest.Executed = true;

							foreach (var exception in message.Exceptions)
							{
								saga.Exceptions.Add(exception);
							}

							var allcommandConfigurationsTested = !saga.CommandConfigurationsToTest.Where(x => !x.Executed).Any();

							if (allcommandConfigurationsTested)
							{
                                saga.RaiseEvent(CompleteTestCommandConfiguration, new TestedAllCommandConfigurationsMessage
								{
									CorrelationId = saga.CorrelationId
								});
							}
						}),
                    When(CompleteTestCommandConfiguration)
						.Publish((saga, message) => new TestedProjectConfigurationMessage
			                {
			                    CorrelationId = saga.RequestorCorrelationId,
                                ResponderCorrelationId = saga.CorrelationId,
				                Exceptions = saga.Exceptions
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

        public ProjectConfigurationCheckerSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

        public virtual Guid RequestorCorrelationId { get; set; }
        public virtual ProjectElement Project { get; set; }
        public virtual IDictionary<string, string> AppSettings { get; set; } 
        public virtual IList<ParallelWorkflowStep<FileMatchElement>> CommandConfigurationsToTest { get; set; }
        public virtual IList<Exception> Exceptions { get; set; }

        public static Event<TestProjectConfigurationMessage> StartTestProjectConfiguration { get; set; }
 
        public static Event<IConfigurationTestResponseMessage> TestedCommandConfiguration { get; set; }
        public static State WaitingForTestedCommandsConfiguration { get; set; }

        public static Event<TestedAllCommandConfigurationsMessage> CompleteTestCommandConfiguration { get; set; }

        private ExportProvider Container
        {
            get { return CurrentConfiguration.Container; }
        }

        private ICommandMessenger GetCommandMessenger(string conversionType)
        {
            var commandConfigurationTesters = Container.GetExportedValues<ICommandMessenger>();
            var commandConfigurationTester = commandConfigurationTesters
                .Where(x => x.Settings.ConversionType == conversionType)
                .First();

            return commandConfigurationTester;
        }

        private void TestCommandConfigurations()
        {
            var commandConfigurationsToTest = new List<FileMatchElement>();

            //We only want to check the sections if they are used, otherwise it will complain about
            //sections missing even if we aren't using them.

            var foldersToWatch = new List<string>();

            //This will check that all the required folders exists
            //It will also check that the service has the correct permissions to create, edit and delete files
            var folderSettings = Project.Folders;
            for (var i = 0; i < folderSettings.Count; i++)
            {
                var folderSetting = folderSettings[i];
                var folderToWatch = folderSetting.GetFolderToWatchOrDefault();

                try
                {
                    //Check that there are no duplicate folderToWatch
                    if (foldersToWatch.Contains(folderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchIsADuplicate, Project.Name, folderSetting.Name, folderToWatch));
                    foldersToWatch.Add(folderToWatch);

                    //Check that folder to watch exists
                    if (!Directory.Exists(folderToWatch)) throw new Exception(string.Format(Resource.ErrorMessageFolderToWatchDoesNotExist, Project.Name, folderSetting.Name, folderToWatch));

                    var workingPath = folderSetting.GetWorkingPathOrDefault();
                    //Check that working path is valid
                    if (!string.IsNullOrEmpty(workingPath))
                    {
                        if (!Directory.Exists(workingPath)) throw new Exception(string.Format(Resource.ErrorMessageWorkingPathDoesNotExist, Project.Name, folderSetting.Name, workingPath));
                        else (new DirectoryInfo(workingPath)).TryCreateTestFile();
                    }
                    else (new DirectoryInfo(Path.GetTempPath())).TryCreateTestFile();

                    var completedPath = folderSetting.GetCompletedPathOrDefault();
                    //Check completed path is valid
                    if (!string.IsNullOrEmpty(completedPath))
                    {
                        if (!Directory.Exists(completedPath)) throw new Exception(string.Format(Resource.ErrorMessageCompletedPathDoesNotExist, Project.Name, folderSetting.Name, completedPath));
                        else (new DirectoryInfo(completedPath)).TryCreateTestFile();
                    }

                    var fileMatches = folderSetting.FileMatches;

                    for (var j = 0; j < fileMatches.Count; j++)
                    {
                        var fileMatch = fileMatches[j];

                        commandConfigurationsToTest.Add(fileMatch);
                    }
                }
                catch (Exception exception)
                {
                    Exceptions.Add(exception);
                }
            }

            CommandConfigurationsToTest = commandConfigurationsToTest
                .Select(x => new ParallelWorkflowStep<FileMatchElement>()
                {
                    CorrelationId = CombGuid.Generate(),
                    Executed = false,
                    MessageInput = x
                })
                .ToList();


            if (Exceptions.Any() || !CommandConfigurationsToTest.Any())
            {
                RaiseEvent(CompleteTestCommandConfiguration, new TestedAllProjectConfigurationsMessage
                    {
                        CorrelationId = CorrelationId
                    });
                return;
            }

            foreach (var parallelWorkflowStep in CommandConfigurationsToTest)
            {
                var commandConfigurationTester = GetCommandMessenger(parallelWorkflowStep.MessageInput.ConversionType);
                var testConfigurationRequestMessage = commandConfigurationTester.CreateTestConfigurationRequestMessage(parallelWorkflowStep.CorrelationId, CorrelationId, AppSettings, Project);

                Bus.Publish(testConfigurationRequestMessage, testConfigurationRequestMessage.GetType());
            }
        }
    }
}
