using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Image.CommandTester.Request;
using Talifun.Commander.Command.Image.CommandTester.Response;
using Talifun.Commander.Command.Image.Configuration;
using Talifun.Commander.Command.Image.Properties;

namespace Talifun.Commander.Command.Image.CommandTester
{
	[Serializable]
	public class ImageConversionConfigurationTesterSaga : SagaStateMachine<ImageConversionConfigurationTesterSaga>, ISaga
	{
		static ImageConversionConfigurationTesterSaga()
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

		public static Event<ImageConversionConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public ImageConversionConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return ImageConversionConfiguration.Instance;
			}
		}

		private void Consume(ImageConversionConfigurationTestRequestMessage message)
		{
			var responseMessage = new ImageConversionConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var imageConversionSettings = message.Configuration;

				var imageConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (imageConversionSettingsKeys.Count > 0)
				{
					var convertPath = message.AppSettings[ImageConversionConfiguration.Instance.ConvertPathSettingName];

					if (string.IsNullOrEmpty(convertPath))
					{
						throw new Exception(string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageAppSettingRequired,
						                                  ImageConversionConfiguration.Instance.ConvertPathSettingName));
					}

					for (var i = 0; i < imageConversionSettings.Count; i++)
					{
						var imageConversionSetting = imageConversionSettings[i];

						var outPutPath = imageConversionSetting.GetOutPutPathOrDefault();
						if (!Directory.Exists(outPutPath))
						{
							throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
								              message.ProjectName,
								              Settings.ElementCollectionSettingName,
								              Settings.ElementSettingName,
								              imageConversionSetting.Name,
								              outPutPath));
						}
						else
						{
							(new DirectoryInfo(outPutPath)).TryCreateTestFile();
						}

						var workingPath = imageConversionSetting.GetWorkingPathOrDefault();
						if (!string.IsNullOrEmpty(workingPath))
						{
							if (!Directory.Exists(workingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              imageConversionSetting.Name,
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

						var errorProcessingPath = imageConversionSetting.GetErrorProcessingPathOrDefault();
						if (!string.IsNullOrEmpty(errorProcessingPath))
						{
							if (!Directory.Exists(errorProcessingPath))
							{
								throw new Exception(
									string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              imageConversionSetting.Name,
									              errorProcessingPath));
							}
							else
							{
								(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
							}
						}

						var watermarkPath = imageConversionSetting.WatermarkPath;
						if (!string.IsNullOrEmpty(watermarkPath))
						{
							if (!File.Exists(watermarkPath))
							{
								throw new Exception(
									string.Format(Resource.ErrorMessageCommandWatermarkPathDoesNotExist,
												  message.ProjectName,
									              Settings.ElementCollectionSettingName,
									              Settings.ElementSettingName,
									              imageConversionSetting.Name,
									              watermarkPath));
							}
						}

						TestImageResizeModeSetting(message.ProjectName, imageConversionSetting);

						imageConversionSettingsKeys.Remove(imageConversionSetting.Name);
					}

					if (imageConversionSettingsKeys.Count > 0)
					{
						FileMatchElement fileMatch = null;
						foreach (var value in imageConversionSettingsKeys.Values)
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

			Bus.MessageContext<ImageConversionConfigurationTestRequestMessage>().Respond(responseMessage); 
		}

		protected void TestImageResizeModeSetting(string projectName, ImageConversionElement image)
		{
			switch (image.ResizeMode)
			{
				case ResizeMode.AreaToFit:
				case ResizeMode.CutToFit:
				case ResizeMode.Zoom:
				case ResizeMode.Stretch:
					if (image.Width == 0)
					{
						throw new Exception(
							string.Format(
								Resource.ErrorMessageWidthIsRequiredForResizeMode,
								projectName,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
								Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
					}
					if (image.Height == 0)
					{
						throw new Exception(
							string.Format(
								Resource.ErrorMessageHeightIsRequiredForResizeMode,
								projectName,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
								Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
					}
					break;
				case ResizeMode.FitWidth:
				case ResizeMode.FitMinimumWidth:
				case ResizeMode.FitMaximumWidth:
					if (image.Width == 0)
					{
						throw new Exception(
							string.Format(
								Resource.ErrorMessageWidthIsRequiredForResizeMode,
								projectName,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
								Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
					}
					break;
				case ResizeMode.FitHeight:
				case ResizeMode.FitMinimumHeight:
				case ResizeMode.FitMaximumHeight:
					if (image.Height == 0)
					{
						throw new Exception(
							string.Format(
								Resource.ErrorMessageHeightIsRequiredForResizeMode,
								projectName,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
								Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
					}
					break;
			}
		}
	}
}
