using System.ComponentModel.Composition;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.FileMatcher.Response
{
	[InheritedExport]
	public interface IPluginResponseMessage : ICommandIdentifier, IWorkflowResponder
	{
	}
}
