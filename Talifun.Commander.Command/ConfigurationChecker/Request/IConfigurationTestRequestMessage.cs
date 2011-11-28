using System.Collections.Generic;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.ConfigurationChecker.Request
{
	[InheritedExport]
	public interface IConfigurationTestRequestMessage : ICommandIdentifier, IWorkflowRequester
	{
		IDictionary<string, string> AppSettings { get; }
	}
}
