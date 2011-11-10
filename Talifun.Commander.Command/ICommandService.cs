using System.ComponentModel.Composition;

namespace Talifun.Commander.Command
{
	[InheritedExport]
	public interface ICommandService
	{
		ISettingConfiguration Settings { get; }
		void Start();
		void Stop();
	}
}