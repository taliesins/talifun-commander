using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Video.CommandTester.Request;
using Talifun.Commander.Command.Video.CommandTester.Response;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video.CommandTester
{
	[Serializable]
	public class VideoConversionConfigurationTesterSaga : SagaStateMachine<VideoConversionConfigurationTesterSaga>, ISaga
	{
		static VideoConversionConfigurationTesterSaga()
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

		public static Event<VideoConversionConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public VideoConversionConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return VideoConversionConfiguration.Instance;
			}
		}

		private void Consume(VideoConversionConfigurationTestRequestMessage message)
		{
			var responseMessage = new VideoConversionConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var videoConversionSettings = message.Configuration;

				var videoConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (videoConversionSettingsKeys.Count > 0)
				{
					var ffMpegPath = message.AppSettings[VideoConversionConfiguration.Instance.FFMpegPathSettingName];

					if (string.IsNullOrEmpty(ffMpegPath))
					{
						throw new Exception(string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageAppSettingRequired,
						                                  VideoConversionConfiguration.Instance.FFMpegPathSettingName));
					}

					var flvTool2Path = message.AppSettings[VideoConversionConfiguration.Instance.FlvTool2PathSettingName];

					if (string.IsNullOrEmpty(flvTool2Path))
					{
						throw new Exception(string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageAppSettingRequired,
						                                  VideoConversionConfiguration.Instance.FlvTool2PathSettingName));
					}

					for (var i = 0; i < videoConversionSettings.Count; i++)
					{
						var videoConversionSetting = videoConversionSettings[i];

						var outPutPath = videoConversionSetting.GetOutPutPathOrDefault();
						if (!Directory.Exists(outPutPath))
						{
							throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
								              message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              videoConversionSetting.Name,
								              outPutPath));
						}
						else
						{
							(new DirectoryInfo(outPutPath)).TryCreateTestFile();
						}

						var workingPath = videoConversionSetting.GetWorkingPathOrDefault();
						if (!string.IsNullOrEmpty(workingPath))
						{
							if (!Directory.Exists(workingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
									              message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              videoConversionSetting.Name,
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

						var errorProcessingPath = videoConversionSetting.GetErrorProcessingPathOrDefault();
						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
									              message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              videoConversionSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						var watermarkPath = videoConversionSetting.WatermarkPath;
						if (!string.IsNullOrEmpty(watermarkPath))
						{
							if (!File.Exists(watermarkPath))
							{
								throw new Exception(
									string.Format(Properties.Resource.ErrorMessageCommandWatermarkPathDoesNotExist,
									              message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              videoConversionSetting.Name,
									              watermarkPath));
							}
						}

						videoConversionSettingsKeys.Remove(videoConversionSetting.Name);
					}

					if (videoConversionSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in videoConversionSettingsKeys.Values)
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

			Bus.MessageContext<VideoConversionConfigurationTestRequestMessage>().Respond(responseMessage); 
		}
	}
}
