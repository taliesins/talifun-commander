using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb
{
	/// <summary>
	/// This interface allows us to create request messages for command plugins, as the core needs to initiate the messages.
	/// </summary>
	[InheritedExport]
	public interface ICommandMessenger
	{
		ISettingConfiguration Settings { get; }
		object CreateCancelMessage(Guid correlationId);
		object CreateRequestMessage(Guid correlationId, Dictionary<string, string> appSettings, ProjectElement project, string workingFilePath, FileMatchElement fileMatch);
		object CreateTestConfigurationRequestMessage(Guid correlationId, Dictionary<string, string> appSettings, ProjectElement project);
	}
}
