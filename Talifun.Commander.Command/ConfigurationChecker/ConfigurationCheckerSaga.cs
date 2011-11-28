using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Magnum;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker.Request;
using Talifun.Commander.Command.ConfigurationChecker.Response;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.ConfigurationChecker
{
	[Serializable]
	public class ConfigurationCheckerSaga : SagaStateMachine<ConfigurationCheckerSaga>, ISaga
	{
		static ConfigurationCheckerSaga()
		{
			Define(() =>
			{
				Initially(
					When(StartTestProjectsConfiguration)
						.Then((saga, message) =>
						{
						    saga.ProjectConfigurationsToTest = saga.GetProjectsToTestConfiguration();
							saga.Exceptions = new List<Exception>();
						})
						.Then((saga, message) => saga.TestProjectConfigurations())
						.TransitionTo(WaitingForTestedProjectConfiguration)
				);

				During(
					WaitingForTestedProjectConfiguration,
					When(TestedProjectConfiguration)
						.Then((saga, message) =>
						{
							var projectConfigurationToTest = saga.ProjectConfigurationsToTest.Where(x => x.CorrelationId == message.ResponderCorrelationId).First();
							projectConfigurationToTest.Executed = true;

							foreach (var exception in message.Exceptions)
							{
								saga.Exceptions.Add(exception);
							}

							var allProjectConfigurationsTested = !saga.ProjectConfigurationsToTest.Where(x => !x.Executed).Any();

							if (allProjectConfigurationsTested)
							{
								saga.RaiseEvent(CompletedTestProjectsConfiguration, new TestedAllProjectConfigurationsMessage
								{
									CorrelationId = saga.CorrelationId
								});
							}
						}),
					When(CompletedTestProjectsConfiguration)
						.Publish((saga, message) => new TestedConfigurationMessage
						{
							CorrelationId = saga.CorrelationId,
							Exceptions = saga.Exceptions
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

		public ConfigurationCheckerSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }
		public virtual IList<ParallelWorkflowStep<ProjectElement>> ProjectConfigurationsToTest { get; set; }
		public virtual IList<Exception> Exceptions { get; set; }

		#region Initialise
		public static Event<TestConfigurationMessage> StartTestProjectsConfiguration { get; set; }
		

		private CommanderSection CommanderSettings
		{
			get { return CurrentConfiguration.CommanderSettings; }
		}

		private AppSettingsSection AppSettings
		{
			get { return CurrentConfiguration.AppSettings; }
		}

		private IList<ParallelWorkflowStep<ProjectElement>> GetProjectsToTestConfiguration()
		{
			return CommanderSettings.Projects
				.Select(x => new ParallelWorkflowStep<ProjectElement>()
				{
					CorrelationId = CombGuid.Generate(),
					Executed = false,
					MessageInput = x
				})
				.ToList();
		}

		private void TestProjectConfigurations()
		{
			if (!ProjectConfigurationsToTest.Any())
			{
				RaiseEvent(CompletedTestProjectsConfiguration, new TestedAllProjectConfigurationsMessage
				{
					CorrelationId = CorrelationId
				});
				return;
			}

			foreach (var parallelWorkflowStep in ProjectConfigurationsToTest)
			{
				var testProjectConfigurationMessage = new TestProjectConfigurationMessage()
				{
					CorrelationId = parallelWorkflowStep.CorrelationId,
					RequestorCorrelationId = CorrelationId,
					Project = parallelWorkflowStep.MessageInput,
					AppSettings = AppSettings.Settings.ToDictionary()
				};

				Bus.Publish(testProjectConfigurationMessage);
			}
		}
		#endregion

		#region Tested Project Configuration
		public static State WaitingForTestedProjectConfiguration { get; set; }
		public static Event<TestedProjectConfigurationMessage> TestedProjectConfiguration { get; set; }
		//public static Event<Fault<TestProjectConfigurationMessage, Guid>> TestProjectConfigurationMessageFailed { get; set; }
		
		#endregion

		#region 
		public static Event<TestedAllProjectConfigurationsMessage> CompletedTestProjectsConfiguration { get; set; }
		#endregion
	}
}
