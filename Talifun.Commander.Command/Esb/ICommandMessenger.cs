using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb.Request;

namespace Talifun.Commander.Command.Esb
{
	/// <summary>
	/// This interface allows us to create request messages for command plugins, as the core needs to initiate the messages.
	/// </summary>
	[InheritedExport]
	public interface ICommandMessenger
	{
		ISettingConfiguration Settings { get; }
		ICommandCancelMessage CreateCancelMessage(Guid correlationId);
		ICommandRequestMessage CreateRequestMessage(Guid correlationId);
		object CreateTestConfigurationRequestMessage(Guid correlationId, Dictionary<string, string> appSettings, ProjectElement project);
	}
}
