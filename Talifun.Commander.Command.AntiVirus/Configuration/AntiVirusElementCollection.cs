using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="AntiVirusElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(AntiVirusElement))]
    public class AntiVirusElementCollection : CurrentConfigurationElementCollection<AntiVirusElement>
    {
        public AntiVirusElementCollection()
        {
            Setting = AntiVirusConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(AntiVirusElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}