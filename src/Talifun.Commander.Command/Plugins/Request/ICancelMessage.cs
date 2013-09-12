using System;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	[InheritedExport]
	public interface ICancelMessage : ICommandIdentifier
	{
		Guid ParentCorrelationId { get; }
	}
}
