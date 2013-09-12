using System;
using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Request;
using Talifun.Commander.Command.FlickrUploader.CommandTester.Request;
using Talifun.Commander.Command.FlickrUploader.Configuration;


namespace Talifun.Commander.Command.FlickrUploader
{
	public class FlickrUploaderMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return FlickrUploaderConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new FlickrUploaderCancelMessage
			{
				CorrelationId = correlationId,
				ParentCorrelationId = requestorCorrelationId
			};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<FlickrUploaderElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new FlickrUploaderRequestMessage
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
			var configuration = project.GetElementCollection<FlickrUploaderElementCollection>(Settings.ElementCollectionSettingName);
			return new FlickrUploaderConfigurationTestRequestMessage
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
