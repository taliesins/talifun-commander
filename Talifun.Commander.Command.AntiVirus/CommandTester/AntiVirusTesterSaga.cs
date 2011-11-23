using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.AntiVirus.CommandTester.Request;
using Talifun.Commander.Command.AntiVirus.CommandTester.Response;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.CommandTester
{
	[Serializable]
	public class AntiVirusTesterSaga : SagaStateMachine<AntiVirusTesterSaga>, ISaga
	{
		static AntiVirusTesterSaga()
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

		public static Event<AntiVirusConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public AntiVirusTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public Guid CorrelationId { get; private set; }
		public IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return AntiVirusConfiguration.Instance;
			}
		}

		private void Consume(AntiVirusConfigurationTestRequestMessage message)
		{
			var responseMessage = new AntiVirusTestConfigurationResponseMessage()
			{
			    CorrelationId = message.CorrelationId
			};

			try
			{
				var commandSettings = new ProjectElementCommand<AntiVirusElementCollection>(Settings.ElementCollectionSettingName, message.Project);
				var antiVirusSettings = commandSettings.Settings;

				var antiVirusSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (antiVirusSettings.Count > 0)
				{
					for (var i = 0; i < antiVirusSettings.Count; i++)
					{
						var antiVirusSetting = antiVirusSettings[i];

						var errorProcessingPath = antiVirusSetting.GetErrorProcessingPathOrDefault();

						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
									              message.Project.Name,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              antiVirusSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						switch (antiVirusSetting.VirusScannerType)
						{
							case VirusScannerType.McAfee:
							case VirusScannerType.NotSpecified:
								{
									var virusScannerPath = message.AppSettings[AntiVirusConfiguration.Instance.McAfeePathSettingName];

									if (string.IsNullOrEmpty(virusScannerPath))
									{
										throw new Exception(string.Format(Commander.Command.Properties.Resource.ErrorMessageAppSettingRequired,
										                                  AntiVirusConfiguration.Instance.McAfeePathSettingName));
									}
								}
								break;
						}

						antiVirusSettingsKeys.Remove(antiVirusSetting.Name);
					}

					if (antiVirusSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in antiVirusSettingsKeys.Values)
						{
							fileMatch = value;
							break;
						}

						throw new Exception(
							string.Format(
								Commander.Command.Properties.Resource.ErrorMessageCommandConversionSettingKeyPointsToNonExistantCommand,
								message.Project.Name, fileMatch.Name, Settings.ElementSettingName, fileMatch.CommandSettingsKey));
					}
				}
			}
			catch (Exception exception)
			{
				responseMessage.Exception = exception;
			}

			Bus.Context().Respond(responseMessage); 
		}
	}
}
