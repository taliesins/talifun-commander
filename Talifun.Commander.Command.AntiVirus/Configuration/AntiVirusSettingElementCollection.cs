using System.Configuration;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="AntiVirusSettingElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(AntiVirusSettingElement))]
    public class AntiVirusSettingElementCollection : CurrentConfigurationElementCollection<AntiVirusSettingElement>
    {
        public AntiVirusSettingElementCollection()
        {
            CollectionSettingName = AntiVirusSettingConfiguration.CollectionSettingName;
            ElementSettingName = AntiVirusSettingConfiguration.ElementSettingName;
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(AntiVirusSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}