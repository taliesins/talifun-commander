using System;
using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.Esb.Response
{
	[InheritedExport]
	public interface ICommandResponseMessage : IEquatable<ICommandResponseMessage>, ICommandIdentifier
	{
	}
}
