using System.Collections.Specialized;
using System.ComponentModel.Composition;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    [InheritedExport]
    public interface ICommandConfigurationTester
    {
        ISettingConfiguration Settings { get; }
        void CheckProjectConfiguration(ProjectElement project, NameValueCollection appSettings);
    }
}