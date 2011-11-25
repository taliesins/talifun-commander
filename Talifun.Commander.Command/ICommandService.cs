using System.ComponentModel.Composition;

namespace Talifun.Commander.Command
{
	[InheritedExport]
	public interface ICommandService
	{
		void Start();
		void Stop();
	}
}