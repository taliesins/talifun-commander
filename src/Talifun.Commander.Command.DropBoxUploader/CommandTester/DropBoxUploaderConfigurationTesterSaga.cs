using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.DropBoxUploader.CommandTester.Request;
using Talifun.Commander.Command.DropBoxUploader.CommandTester.Response;
using Talifun.Commander.Command.DropBoxUploader.Configuration;

namespace Talifun.Commander.Command.DropBoxUploader.CommandTester
{
	[Serializable]
	public class DropBoxUploaderConfigurationTesterSaga : SagaStateMachine<DropBoxUploaderConfigurationTesterSaga>, ISaga
	{
		static DropBoxUploaderConfigurationTesterSaga()
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

		public static Event<DropBoxUploaderConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public DropBoxUploaderConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return DropBoxUploaderConfiguration.Instance;
			}
		}

		private void Consume(DropBoxUploaderConfigurationTestRequestMessage message)
		{
			var responseMessage = new DropBoxUploaderConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var flickrUploaderSettings = message.Configuration;

				var flickrUploaderSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (flickrUploaderSettingsKeys.Count > 0)
				{

					for (var i = 0; i < flickrUploaderSettings.Count; i++)
					{
						var flickrUploaderSetting = flickrUploaderSettings[i];

						var outPutPath = flickrUploaderSetting.GetOutPutPathOrDefault();
						if (!Directory.Exists(outPutPath))
						{
							throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
								              message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              flickrUploaderSetting.Name,
								              outPutPath));
						}
						else
						{
							(new DirectoryInfo(outPutPath)).TryCreateTestFile();
						}

						var workingPath = flickrUploaderSetting.GetWorkingPathOrDefault();
						if (!string.IsNullOrEmpty(workingPath))
						{
							if (!Directory.Exists(workingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              flickrUploaderSetting.Name,
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

						var errorProcessingPath = flickrUploaderSetting.GetErrorProcessingPathOrDefault();
						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              flickrUploaderSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						flickrUploaderSettingsKeys.Remove(flickrUploaderSetting.Name);
					}

					if (flickrUploaderSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in flickrUploaderSettingsKeys.Values)
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

			Bus.MessageContext<DropBoxUploaderConfigurationTestRequestMessage>().Respond(responseMessage); 
		}
	}
}
