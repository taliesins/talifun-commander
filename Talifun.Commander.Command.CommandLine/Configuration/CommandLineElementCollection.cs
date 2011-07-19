using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="CommandLineElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(CommandLineElement))]
    public class CommandLineElementCollection : CurrentConfigurationElementCollection<CommandLineElement>
    {
        public CommandLineElementCollection()
        {
            Setting = CommandLineConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(CommandLineElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
