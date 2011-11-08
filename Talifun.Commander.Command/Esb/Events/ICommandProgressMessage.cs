using System;
using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.Esb.Events
{
	[InheritedExport]
	public interface ICommandProgressMessage : IEquatable<ICommandProgressMessage>, ICommandIdentifier
	{
	}
}
