using System;
using System.Collections.Generic;
using System.IO;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Audio.CommandTester.Request;
using Talifun.Commander.Command.Audio.CommandTester.Response;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio.CommandTester
{
	[Serializable]
	public class AudioConversionConfigurationTesterSaga : SagaStateMachine<AudioConversionConfigurationTesterSaga>, ISaga
	{
		static AudioConversionConfigurationTesterSaga()
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

		public static Event<AudioConversionConfigurationTestRequestMessage> TestConfigurationRequestEvent { get; set; }

		public AudioConversionConfigurationTesterSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual Guid CorrelationId { get; private set; }
		public virtual IServiceBus Bus { get; set; }

		private ISettingConfiguration Settings
		{
			get
			{
				return AudioConversionConfiguration.Instance;
			}
		}

		private void Consume(AudioConversionConfigurationTestRequestMessage message)
		{
			var responseMessage = new AudioConversionConfigurationTestResponseMessage()
			{
			    CorrelationId = message.RequestorCorrelationId,
				ResponderCorrelationId = message.CorrelationId,
				Exceptions = new List<Exception>()
			};

			try
			{
				var audioConversionSettings = message.Configuration;

				var audioConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

				if (audioConversionSettingsKeys.Count <= 0)
				{
					return;
				}

				var ffMpegPath = message.AppSettings[AudioConversionConfiguration.Instance.FFMpegPathSettingName];

				if (string.IsNullOrEmpty(ffMpegPath))
				{
					throw new Exception(string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageAppSettingRequired, AudioConversionConfiguration.Instance.FFMpegPathSettingName));
				}

            
				for (var i = 0; i < audioConversionSettings.Count; i++)
				{
					var audioConversionSetting = audioConversionSettings[i];

            		var outPutPath = audioConversionSetting.GetOutPutPathOrDefault();

					if (!Directory.Exists(outPutPath))
					{
                		throw new Exception(
							string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
										  message.ProjectName,
                						  Settings.ElementCollectionSettingName,
                						  Settings.ElementSettingName,
                						  audioConversionSetting.Name,
										  outPutPath));
					}
					else
					{
						(new DirectoryInfo(outPutPath)).TryCreateTestFile();
					}

            		var workingPath = audioConversionSetting.GetWorkingPathOrDefault();
					if (!string.IsNullOrEmpty(workingPath))
					{
						if (!Directory.Exists(workingPath))
						{
                    		throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    						  message.ProjectName,
                    						  Settings.ElementCollectionSettingName,
                    						  Settings.ElementSettingName,
                    						  audioConversionSetting.Name,
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

            		var errorProcessingPath = audioConversionSetting.GetErrorProcessingPathOrDefault();
					if (!string.IsNullOrEmpty(errorProcessingPath))
					{
						if (!Directory.Exists(errorProcessingPath))
						{
                    		throw new Exception(
								string.Format(Talifun.Commander.Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    						  message.ProjectName,
                    						  Settings.ElementCollectionSettingName,
                    						  Settings.ElementSettingName,
                    						  audioConversionSetting.Name,
											  errorProcessingPath));
						}
						else
						{
							(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
						}
					}

					audioConversionSettingsKeys.Remove(audioConversionSetting.Name);
				}

				if (audioConversionSettingsKeys.Count > 0)
				{
					FileMatchElement fileMatch = null;
					foreach (var value in audioConversionSettingsKeys.Values)
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
			catch (Exception exception)
			{
				responseMessage.Exceptions.Add(exception);
			}

			Bus.MessageContext<AudioConversionConfigurationTestRequestMessage>().Respond(responseMessage); 
		}
	}
}
