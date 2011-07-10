using System.Configuration;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="CommandLineSettingElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(CommandLineSettingElement))]
    public class CommandLineSettingElementCollection : CurrentConfigurationElementCollection<CommandLineSettingElement>
    {
        public CommandLineSettingElementCollection()
        {
            Setting = CommandLineConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(CommandLineSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
