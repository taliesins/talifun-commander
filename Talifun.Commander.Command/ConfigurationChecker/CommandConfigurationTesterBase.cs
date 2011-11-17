using System.Configuration;

namespace Talifun.Commander.Command.ConfigurationChecker
{
    public abstract class CommandConfigurationTesterBase : ICommandConfigurationTester
    {
        public abstract void CheckProjectConfiguration(AppSettingsSection appSettings, Configuration.ProjectElement project);

        public abstract ISettingConfiguration Settings { get; }
    }
}