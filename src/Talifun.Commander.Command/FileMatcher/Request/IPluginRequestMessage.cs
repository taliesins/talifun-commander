using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Request
{
	[InheritedExport]
	public interface IPluginRequestMessage : ICommandIdentifier, IWorkflowRequester
	{
		string InputFilePath { get; }
		FileMatchElement FileMatch { get; }
	}
}
