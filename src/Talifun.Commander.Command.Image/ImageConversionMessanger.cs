using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.CommandTester.Request;
using Talifun.Commander.Command.Image.Configuration;

namespace Talifun.Commander.Command.Image
{
	public class ImageConversionMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return ImageConversionConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new ImageConversionCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<ImageConversionElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new ImageConversionRequestMessage
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
			var configuration = project.GetElementCollection<ImageConversionElementCollection>(Settings.ElementCollectionSettingName);
			return new ImageConversionConfigurationTestRequestMessage
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
