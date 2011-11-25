using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface IConfigurationTestRequestMessage : ICommandIdentifier
	{
		Guid CorrelationId { get; }
		Dictionary<string, string> AppSettings { get; }
	}
}
