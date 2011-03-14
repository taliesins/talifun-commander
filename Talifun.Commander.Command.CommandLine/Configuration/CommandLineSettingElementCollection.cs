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
            CollectionSettingName = CommandLineSettingConfiguration.Instance.CollectionSettingName;
            ElementSettingName = CommandLineSettingConfiguration.Instance.ElementSettingName;
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(CommandLineSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
