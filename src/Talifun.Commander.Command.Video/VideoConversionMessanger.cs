using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.CommandTester.Request;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video
{
	public class VideoConversionMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return VideoConversionConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new VideoConversionCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<VideoConversionElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new VideoConversionRequestMessage
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
			var configuration = project.GetElementCollection<VideoConversionElementCollection>(Settings.ElementCollectionSettingName);
			return new VideoConversionConfigurationTestRequestMessage
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
