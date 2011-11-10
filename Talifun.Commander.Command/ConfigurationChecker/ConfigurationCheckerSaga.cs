using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;
using Talifun.Commander.Command.ConfigurationChecker.Messages;

namespace Talifun.Commander.Command.Esb
{
	[Serializable]
	public class TestConfigurationSaga : SagaStateMachine<TestConfigurationSaga>, ISaga
	{
		static TestConfigurationSaga()
		{
			Define(() =>
			{
				Initially(
					When(TestConfigurationEvent)
						.Then((saga, message) => saga.CheckConfiguration(message))
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

		public static Event<RequestTestConfigurationMessage> TestConfigurationEvent { get; set; }

		public TestConfigurationSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public Guid CorrelationId { get; private set; }
		public IServiceBus Bus { get; set; }

		#region Test Configuration

		protected virtual ExportProvider Container
		{
			get { return CurrentConfiguration.Container; }
		}

		protected virtual CommanderSection CommanderSettings
		{
			get { return CurrentConfiguration.CommanderSettings; }
		}

		protected virtual AppSettingsSection AppSettings
		{
			get { return CurrentConfiguration.AppSettings; }
		}

		private void CheckConfiguration(RequestTestConfigurationMessage message)
		{
			var commandConfigurationTester = new CommandConfigurationTester(Container);

			var projects = CommanderSettings.Projects;
			for (var j = 0; j < projects.Count; j++)
			{
				commandConfigurationTester.CheckProjectConfiguration(AppSettings, projects[j]);
			}

			var responseTestConfigurationMessage = new ResponseTestConfigurationMessage
			                                       	{
			                                       		CorrelationId = message.CorrelationId
			                                       	};

			Bus.Publish(responseTestConfigurationMessage);
		}

		#endregion
	}
}
