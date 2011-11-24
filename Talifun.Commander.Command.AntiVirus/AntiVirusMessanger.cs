using System;
using System.Collections.Generic;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.CommandTester.Request;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.AntiVirus
{
	public class AntiVirusMessanger : ICommandMessenger
	{
		public ISettingConfiguration Settings
		{
			get { return AntiVirusConfiguration.Instance; }
		}

		public ICommandCancelMessage CreateCancelMessage(Guid correlationId)
		{
			return new AntiVirusCancelMessage
			       	{
						CorrelationId = correlationId
			       	};
		}

		public ICommandRequestMessage CreateRequestMessage(Guid correlationId, Dictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch)
		{
			return new AntiVirusRequestMessage
			       	{
						CorrelationId = correlationId,
			       		AppSettings = appSettings,
			       		Project = project,
						WorkingFilePath = workingFilePath,
						FileMatch = fileMatch
			       	};
		}

		public object CreateTestConfigurationRequestMessage(Guid correlationId, Dictionary<string, string> appSettings, ProjectElement project)
		{
			return new AntiVirusConfigurationTestRequestMessage
			       	{
						CorrelationId = correlationId,
			       		AppSettings = appSettings,
			       		Project = project
			       	};
		}
	}
}
