using System.ComponentModel.Composition;

namespace Talifun.Commander.Command.FileMatcher
{
    [InheritedExport]
    public interface ICommandSaga
    {
        ISettingConfiguration Settings { get; }
        void Run(ICommandSagaProperties properties);
    }
}