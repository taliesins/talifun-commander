using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.ConfigurationChecker.Response
{
	[InheritedExport]
	public interface IConfigurationTestResponseMessage : ICommandIdentifier, IWorkflowResponder
	{
		IList<Exception> Exceptions { get; set; }
	}
}
