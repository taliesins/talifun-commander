using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="FileMatchElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(FileMatchElement))]
    public class FileMatchElementCollection : CurrentConfigurationElementCollection<FileMatchElement>
    {
        public FileMatchElementCollection()
        {
            CollectionSettingName = "fileMatches";
            ElementSettingName = "fileMatch";
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(FileMatchElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}