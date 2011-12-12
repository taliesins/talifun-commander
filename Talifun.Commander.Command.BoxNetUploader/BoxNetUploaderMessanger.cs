using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.BoxNetUploader.Command.Request;
using Talifun.Commander.Command.BoxNetUploader.CommandTester.Request;
using Talifun.Commander.Command.BoxNetUploader.Configuration;


namespace Talifun.Commander.Command.BoxNetUploader
{
	public class BoxNetUploaderMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return BoxNetUploaderConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new BoxNetUploaderCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<BoxNetUploaderElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new BoxNetUploaderRequestMessage
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
			var configuration = project.GetElementCollection<BoxNetUploaderElementCollection>(Settings.ElementCollectionSettingName);
			return new BoxNetUploaderConfigurationTestRequestMessage
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
