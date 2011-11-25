using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
	public class ProjectElementCommand<T> where T : CurrentConfigurationElementCollection
    {
        protected readonly ProjectElement ProjectElement;
        protected readonly ConfigurationProperty CommandSettings;

        public ProjectElementCommand(string elementCollectionSettingName, ProjectElement projectElement)
        {
            ProjectElement = projectElement;
            CommandSettings = new ConfigurationProperty(elementCollectionSettingName, typeof (T), null, ConfigurationPropertyOptions.None);
            ProjectElement.AddElementCollection(CommandSettings);
        }

        public T Settings
        {
            get { return ProjectElement.GetElementCollection<T>(CommandSettings); }
        }
    }
}
