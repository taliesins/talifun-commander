using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command.Request;
using Talifun.Commander.Command.PicasaUploader.CommandTester.Request;
using Talifun.Commander.Command.PicasaUploader.Configuration;

namespace Talifun.Commander.Command.PicasaUploader
{
	public class PicasaUploaderMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return PicasaUploaderConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new PicasaUploaderCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<PicasaUploaderElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new PicasaUploaderRequestMessage
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
			var configuration = project.GetElementCollection<PicasaUploaderElementCollection>(Settings.ElementCollectionSettingName);
			return new PicasaUploaderConfigurationTestRequestMessage
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
