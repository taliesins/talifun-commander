using System.Configuration;

namespace Talifun.Commander.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="FileMatchElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(FileMatchElement))]
    public sealed class FileMatchElementCollection : CurrentConfigurationElementCollection<FileMatchElement>
    {
        public FileMatchElementCollection()
        {
            AddElementName = "fileMatch";
        }
    }
}