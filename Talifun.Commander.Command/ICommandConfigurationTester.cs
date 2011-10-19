using System.ComponentModel.Composition;
using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    [InheritedExport]
    public interface ICommandConfigurationTester
    {
        ISettingConfiguration Settings { get; }
        void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project);
    }
}