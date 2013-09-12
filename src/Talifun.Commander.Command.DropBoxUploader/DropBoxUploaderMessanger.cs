using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.DropBoxUploader.Command.Request;
using Talifun.Commander.Command.DropBoxUploader.CommandTester.Request;
using Talifun.Commander.Command.DropBoxUploader.Configuration;


namespace Talifun.Commander.Command.DropBoxUploader
{
	public class DropBoxUploaderMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return DropBoxUploaderConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new DropBoxUploaderCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<DropBoxUploaderElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new DropBoxUploaderRequestMessage
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
			var configuration = project.GetElementCollection<DropBoxUploaderElementCollection>(Settings.ElementCollectionSettingName);
			return new DropBoxUploaderConfigurationTestRequestMessage
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
