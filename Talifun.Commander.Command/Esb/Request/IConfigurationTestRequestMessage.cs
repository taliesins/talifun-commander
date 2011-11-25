using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface IConfigurationTestRequestMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; }
		ProjectElement Project { get; }
		Dictionary<string, string> AppSettings { get; }
	}
}
