using System;
using System.Collections.Generic;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.CommandTester.Request;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return AntiVirusConfiguration.Instance; }
		}

		public object CreateCancelMessage(Guid correlationId, Guid requestorCorrelationId)
		{
			return new AntiVirusCancelMessage
			       	{
						CorrelationId = correlationId,
						ParentCorrelationId = requestorCorrelationId
			       	};
		}

		public object CreateRequestMessage(Guid correlationId, Guid requestorCorrelationId, IDictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			var configuration = project.GetElement<AntiVirusElement>(fileMatch, Settings.ElementCollectionSettingName);

			return new AntiVirusRequestMessage
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
			var configuration = project.GetElementCollection<AntiVirusElementCollection>(Settings.ElementCollectionSettingName);
			return new AntiVirusConfigurationTestRequestMessage
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
