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
            Setting = ProjectConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.CollectionSettingName, typeof(ProjectElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
