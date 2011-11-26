using System;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface IPluginRequestMessage : ICommandIdentifier
	{
		Guid ParentCorrelationId { get; }
		string InputFilePath { get; }
		FileMatchElement FileMatch { get; }
	}
}
