using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.CommandTester.Request;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio
{
	public class AudioConversionMessenger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return AudioConversionConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new AudioConversionCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<AudioConversionElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new AudioConversionRequestMessage
			{
				CorrelationId = correlationId,
				RequestorCorrelationId = requestorCorrelationId,
				AppSettings = appSettings,
				Configuration = configuration,
				InputFilePath = workingFilePath,
				FileMatch = fileMatch
			};
		}

		public object CreateTestConfigurationRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project)
		{
			var configuration = project.GetElementCollection<AudioConversionElementCollection>(Settings.ElementCollectionSettingName);
			return new AudioConversionConfigurationTestRequestMessage
			{
				CorrelationId = correlationId,
				RequestorCorrelationId = requestorCorrelationId,
				AppSettings = appSettings,
				ProjectName = project.Name,
				Configuration = configuration
			};
		}
	}
}
