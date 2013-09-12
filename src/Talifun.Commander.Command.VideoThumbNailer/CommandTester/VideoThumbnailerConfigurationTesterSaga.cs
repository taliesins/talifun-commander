using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.VideoThumbNailer.CommandTester.Request;
using Talifun.Commander.Command.VideoThumbNailer.CommandTester.Response;
using Talifun.Commander.Command.VideoThumbnailer.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer.CommandTester
{
	[Serializable]
	public class VideoThumbnailerConfigurationTesterSaga : SagaStateMachine<VideoThumbnailerConfigurationTesterSaga>, ISaga
	{
		static VideoThumbnailerConfigurationTesterSaga()
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

		public static Event<VideoThumbnailerConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public VideoThumbnailerConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return VideoThumbnailerConfiguration.Instance;
			}
		}

		private void Consume(VideoThumbnailerConfigurationTestRequestMessage message)
		{
			var responseMessage = new VideoThumbnailerConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var videoThumbnailerSettings = message.Configuration;

				var videoThumbnailerSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (videoThumbnailerSettingsKeys.Count > 0)
				{

					var ffMpegPath = message.AppSettings[VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName];

					if (string.IsNullOrEmpty(ffMpegPath))
					{
						throw new Exception(string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageAppSettingRequired,
						                                  VideoThumbnailerConfiguration.Instance.FFMpegPathSettingName));
					}

					for (var i = 0; i < videoThumbnailerSettings.Count; i++)
					{
						var videoThumbnailerSetting = videoThumbnailerSettings[i];

						var outPutPath = videoThumbnailerSetting.GetOutPutPathOrDefault();
						if (!Directory.Exists(outPutPath))
						{
							throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
											  message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              videoThumbnailerSetting.Name,
								              outPutPath));
						}
						else
						{
							(new DirectoryInfo(outPutPath)).TryCreateTestFile();
						}

						var workingPath = videoThumbnailerSetting.GetWorkingPathOrDefault();
						if (!string.IsNullOrEmpty(workingPath))
						{
							if (!Directory.Exists(workingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              videoThumbnailerSetting.Name,
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

						var errorProcessingPath = videoThumbnailerSetting.GetErrorProcessingPathOrDefault();
						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              videoThumbnailerSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						videoThumbnailerSettingsKeys.Remove(videoThumbnailerSetting.Name);
					}

					if (videoThumbnailerSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in videoThumbnailerSettingsKeys.Values)
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

			Bus.MessageContext<VideoThumbnailerConfigurationTestRequestMessage>().Respond(responseMessage); 
		}
	}
}
