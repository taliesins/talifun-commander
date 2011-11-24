using System;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Events
{
	[InheritedExport]
	public interface ICommandStartedMessage : IEquatable<ICommandStartedMessage>, ICommandIdentifier
	{
		string WorkingFilePath { get; }
		FileMatchElement FileMatch { get; }
	}
}
