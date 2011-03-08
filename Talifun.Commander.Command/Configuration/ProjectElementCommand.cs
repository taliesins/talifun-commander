using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    public class ProjectElementCommand<T> where T : ConfigurationElementCollection
    {
        protected readonly ProjectElement ProjectElement;
        protected readonly ConfigurationProperty CommandSettings;

        public ProjectElementCommand(string configurationElementName, ProjectElement projectElement)
        {
            ProjectElement = projectElement;
            CommandSettings = new ConfigurationProperty(configurationElementName, typeof (T), null, ConfigurationPropertyOptions.None);
            ProjectElement.AddCommandConfiguration(CommandSettings);
        }

        public T Settings
        {
            get { return ProjectElement.GetCommandConfiguration<T>(CommandSettings); }
        }
    }
}
