using System;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Events
{
	[InheritedExport]
	public interface ICommandCompletedMessage : IEquatable<ICommandCompletedMessage>, ICommandIdentifier
	{
		string InputFilePath { get; }
		FileMatchElement FileMatch { get; }
	}
}
