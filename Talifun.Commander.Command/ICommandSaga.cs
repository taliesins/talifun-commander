using System.ComponentModel.Composition;

namespace Talifun.Commander.Command
{
    [InheritedExport]
    public interface ICommandSaga
    {
        string ConversionType { get; }
        void Run(ICommandSagaProperties properties);
    }
}