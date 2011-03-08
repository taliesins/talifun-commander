using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="ProjectElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(ProjectElement))]
    public class ProjectElementCollection : CurrentConfigurationElementCollection<ProjectElement>
    {
        public ProjectElementCollection()
        {
            CollectionSettingName = "projects";
            ElementSettingName = "project";
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(ProjectElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
