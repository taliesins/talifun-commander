using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.YouTubeUploader.CommandTester.Request;
using Talifun.Commander.Command.YouTubeUploader.CommandTester.Response;
using Talifun.Commander.Command.YouTubeUploader.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader.CommandTester
{
	[Serializable]
	public class YouTubeUploaderConfigurationTesterSaga : SagaStateMachine<YouTubeUploaderConfigurationTesterSaga>, ISaga
	{
		static YouTubeUploaderConfigurationTesterSaga()
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

		public static Event<YouTubeUploaderConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public YouTubeUploaderConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return YouTubeUploaderConfiguration.Instance;
			}
		}

		private void Consume(YouTubeUploaderConfigurationTestRequestMessage message)
		{
			var responseMessage = new YouTubeUploaderConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var youTubeUploaderSettings = message.Configuration;

				var youTubeUploaderSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (youTubeUploaderSettingsKeys.Count > 0)
				{

					for (var i = 0; i < youTubeUploaderSettings.Count; i++)
					{
						var youTubeUploaderSetting = youTubeUploaderSettings[i];

						var outPutPath = youTubeUploaderSetting.GetOutPutPathOrDefault();
						if (!Directory.Exists(outPutPath))
						{
							throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
								              message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              youTubeUploaderSetting.Name,
								              outPutPath));
						}
						else
						{
							(new DirectoryInfo(outPutPath)).TryCreateTestFile();
						}

						var workingPath = youTubeUploaderSetting.GetWorkingPathOrDefault();
						if (!string.IsNullOrEmpty(workingPath))
						{
							if (!Directory.Exists(workingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              youTubeUploaderSetting.Name,
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

						var errorProcessingPath = youTubeUploaderSetting.GetErrorProcessingPathOrDefault();
						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              youTubeUploaderSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						youTubeUploaderSettingsKeys.Remove(youTubeUploaderSetting.Name);
					}

					if (youTubeUploaderSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in youTubeUploaderSettingsKeys.Values)
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

			Bus.MessageContext<YouTubeUploaderConfigurationTestRequestMessage>().Respond(responseMessage); 
		}
	}
}
