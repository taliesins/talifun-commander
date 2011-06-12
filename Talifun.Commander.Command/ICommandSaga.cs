using System.ComponentModel.Composition;

namespace Talifun.Commander.Command
{
    [InheritedExport]
    public interface ICommandSaga
    {
        ISettingConfiguration Settings { get; }
        void Run(ICommandSagaProperties properties);
    }
}