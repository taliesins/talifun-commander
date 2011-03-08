using System.Configuration;

namespace Talifun.Commander.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="ProjectElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(ProjectElement))]
    public sealed class ProjectElementCollection : CurrentConfigurationElementCollection<ProjectElement>
    {
        public ProjectElementCollection()
        {
            AddElementName = "project";
        }
    }
}
