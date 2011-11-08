using System;
using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICommandCancelMessage : IEquatable<ICommandCancelMessage>, ICommandIdentifier
	{
	}
}
