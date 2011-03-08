using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="FolderElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(FolderElement))]
    public class FolderElementCollection : CurrentConfigurationElementCollection<FolderElement>
    {
        public FolderElementCollection()
        {
            CollectionSettingName = "folders";
            ElementSettingName = "folder";
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(FolderElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
