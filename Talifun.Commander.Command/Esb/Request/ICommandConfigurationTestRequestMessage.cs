using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICommandConfigurationTestRequestMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; set; }
		ProjectElement Project { get; set; }
		Dictionary<string, string> AppSettings { get; set; }
	}
}
