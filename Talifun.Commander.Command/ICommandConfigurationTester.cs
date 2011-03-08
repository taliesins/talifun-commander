using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    [InheritedExport]
    public interface ICommandConfigurationTester
    {
        string ConversionType { get; }
        void CheckProjectConfiguration(ProjectElement project);
    }
}