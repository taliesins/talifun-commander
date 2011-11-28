using System;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Events
{
	[InheritedExport]
	public interface IPluginProgressMessage : IEquatable<IPluginProgressMessage>, ICommandIdentifier
	{
		string InputFilePath { get; }
		FileMatchElement FileMatch { get; }
	}
}
