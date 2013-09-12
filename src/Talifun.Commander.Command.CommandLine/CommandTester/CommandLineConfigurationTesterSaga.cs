using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.CommandLine.CommandTester.Request;
using Talifun.Commander.Command.CommandLine.CommandTester.Response;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.CommandLine.Properties;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.CommandTester
{
	[Serializable]
	public class CommandLineConfigurationTesterSaga : SagaStateMachine<CommandLineConfigurationTesterSaga>, ISaga
	{
		static CommandLineConfigurationTesterSaga()
		{
			Define(() =>
			{
				Initially(
					When(TestConfigurationRequestEvent)
						.Then((saga, message) =>
						      	{
						      		saga.Consume(message);
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

		public static Event<CommandLineConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public CommandLineConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return CommandLineConfiguration.Instance;
			}
		}

		private void Consume(CommandLineConfigurationTestRequestMessage message)
		{
			var responseMessage = new CommandLineConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var commandLineSettings = message.Configuration;
				var commandLineSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (commandLineSettingsKeys.Count > 0)
				{
					for (var i = 0; i < commandLineSettings.Count; i++)
					{
						var commandLineSetting = commandLineSettings[i];

						var commandPath = commandLineSetting.CommandPath;
						if (commandLineSetting.CheckCommandPathExists && !File.Exists(commandPath))
						{
							throw new Exception(
								string.Format(Resource.ErrorMessageCommandPathDoesNotExist,
								              message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              commandLineSetting.Name,
								              commandPath));
						}

						var outPutPath = commandLineSetting.GetOutPutPathOrDefault();
						if (!Directory.Exists(outPutPath))
						{
							throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
								              message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              commandLineSetting.Name,
								              outPutPath));
						}
						else
						{
							(new DirectoryInfo(outPutPath)).TryCreateTestFile();
						}

						var workingPath = commandLineSetting.GetWorkingPathOrDefault();
						if (!string.IsNullOrEmpty(workingPath))
						{
							if (!Directory.Exists(workingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
									              message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              commandLineSetting.Name,
									              workingPath));
							}
							else
							{
								(new DirectoryInfo(workingPath)).TryCreateTestFile();
							}
						}
						else
						{
							(new DirectoryInfo(Path.GetTempPath())).TryCreateTestFile();
						}

						var errorProcessingPath = commandLineSetting.GetErrorProcessingPathOrDefault();
						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
									              message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              commandLineSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						commandLineSettingsKeys.Remove(commandLineSetting.Name);
					}

					if (commandLineSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in commandLineSettingsKeys.Values)
						{
							fileMatch = value;
							break;
						}

						throw new Exception(
							string.Format(
								Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandConversionSettingKeyPointsToNonExistantCommand,
								message.ProjectName, fileMatch.Name, Settings.ElementSettingName, fileMatch.CommandSettingsKey));
					}
				}
			}
			catch (Exception exception)
			{
				responseMessage.Exceptions.Add(exception);
			}

			Bus.MessageContext<CommandLineConfigurationTestRequestMessage>().Respond(responseMessage); 
		}
	}
}
