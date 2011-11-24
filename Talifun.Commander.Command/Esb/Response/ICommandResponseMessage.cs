using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb.Response
{
	[InheritedExport]
	public interface ICommandResponseMessage : ICommandIdentifier
	{
		string WorkingFilePath { get; }
		FileMatchElement FileMatch { get; }
	}
}
