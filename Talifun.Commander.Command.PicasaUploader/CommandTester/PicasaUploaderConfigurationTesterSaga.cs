using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.PicasaUploader.CommandTester.Request;
using Talifun.Commander.Command.PicasaUploader.CommandTester.Response;
using Talifun.Commander.Command.PicasaUploader.Configuration;

namespace Talifun.Commander.Command.PicasaUploader.CommandTester
{
	[Serializable]
	public class PicasaUploaderConfigurationTesterSaga : SagaStateMachine<PicasaUploaderConfigurationTesterSaga>, ISaga
	{
		static PicasaUploaderConfigurationTesterSaga()
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

		public static Event<PicasaUploaderConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public PicasaUploaderConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return PicasaUploaderConfiguration.Instance;
			}
		}

		private void Consume(PicasaUploaderConfigurationTestRequestMessage message)
		{
			var responseMessage = new PicasaUploaderConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var picasaUploaderSettings = message.Configuration;

				var picasaUploaderSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (picasaUploaderSettingsKeys.Count > 0)
				{

					for (var i = 0; i < picasaUploaderSettings.Count; i++)
					{
						var picasaUploaderSetting = picasaUploaderSettings[i];

						var outPutPath = picasaUploaderSetting.GetOutPutPathOrDefault();
						if (!Directory.Exists(outPutPath))
						{
							throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
								              message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              picasaUploaderSetting.Name,
								              outPutPath));
						}
						else
						{
							(new DirectoryInfo(outPutPath)).TryCreateTestFile();
						}

						var workingPath = picasaUploaderSetting.GetWorkingPathOrDefault();
						if (!string.IsNullOrEmpty(workingPath))
						{
							if (!Directory.Exists(workingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              picasaUploaderSetting.Name,
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

						var errorProcessingPath = picasaUploaderSetting.GetErrorProcessingPathOrDefault();
						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              picasaUploaderSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						picasaUploaderSettingsKeys.Remove(picasaUploaderSetting.Name);
					}

					if (picasaUploaderSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in picasaUploaderSettingsKeys.Values)
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

			Bus.MessageContext<PicasaUploaderConfigurationTestRequestMessage>().Respond(responseMessage); 
		}
	}
}
