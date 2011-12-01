using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.YouTubeUploader.Command.Request;
using Talifun.Commander.Command.YouTubeUploader.CommandTester.Request;
using Talifun.Commander.Command.YouTubeUploader.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader
{
	public class YouTubeUploaderMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return YouTubeUploaderConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new YouTubeUploaderCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<YouTubeUploaderElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new YouTubeUploaderRequestMessage
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
			var configuration = project.GetElementCollection<YouTubeUploaderElementCollection>(Settings.ElementCollectionSettingName);
			return new YouTubeUploaderConfigurationTestRequestMessage
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
