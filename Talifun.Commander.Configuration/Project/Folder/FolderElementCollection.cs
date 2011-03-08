using System.Configuration;

namespace Talifun.Commander.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="FolderElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(FolderElement))]
    public sealed class FolderElementCollection : CurrentConfigurationElementCollection<FolderElement>
    {
        public FolderElementCollection()
        {
            AddElementName = "folder";
        }
    }
}
