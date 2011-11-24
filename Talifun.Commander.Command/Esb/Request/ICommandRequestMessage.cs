using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Request
{
	[InheritedExport]
	public interface ICommandRequestMessage : ICommandIdentifier
	{
		string WorkingFilePath { get; }
		FileMatchElement FileMatch { get; }
	}
}
