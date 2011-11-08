using System;
using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICommandRequestMessage : IEquatable<ICommandRequestMessage>, ICommandIdentifier
	{
	}
}
