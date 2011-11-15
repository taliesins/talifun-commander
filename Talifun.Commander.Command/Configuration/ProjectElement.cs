using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Talifun.Commander.Command.Configuration
{
    public partial class ProjectElement : NamedConfigurationElement
    {
		private readonly string[] _excludedElements = new[] { "project", "folder", "fileMatch" };

        protected ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        protected readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        protected readonly ConfigurationProperty folders = new ConfigurationProperty("folders", typeof(FolderElementCollection), new FolderElementCollection(), ConfigurationPropertyOptions.None);
        
        /// <summary>
        /// Initializes the <see cref="ProjectElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        public ProjectElement()
        {
			Setting = ProjectConfiguration.Instance;	

            var container = CommandContainer.Instance.Container;

        	var elementCollectionConfigurationProperties = container.GetExportedValues<CurrentConfigurationElementCollection>()
        		.Where(x => !_excludedElements.Contains(x.Setting.ElementSettingName));
  
            properties.Add(name);
            properties.Add(folders);

            foreach (var elementCollectionConfigurationProperty in elementCollectionConfigurationProperties)
            {
                properties.Add(elementCollectionConfigurationProperty.CreateNewConfigurationProperty());
            }
        }

        /// <summary>
        /// Gets or sets the name of the configuration element represented by this instance.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = null, IsRequired = true, IsKey = true)]
        public override string Name
        {
            get { return ((string)base[name]); }
            set { SetPropertyValue(value, name, "Name"); }
        }

        /// <summary>
        /// Gets a <see cref="FolderElementCollection" /> containing the <see cref="FolderElement" /> elements
        /// for the folders to watch.
        /// </summary>
        /// <value>A <see cref="ProviderSettingsCollection" /> containing the configuration elements associated with this configuration section.</value>
        [ConfigurationProperty("folders", DefaultValue = null, IsDefaultCollection = true)]
        public FolderElementCollection Folders
        {
            get { return ((FolderElementCollection)base[folders]); }
        }

		public List<CurrentConfigurationElementCollection> CommandPlugins
		{
			get
			{
				var commandConfigurations = properties.Cast<ConfigurationProperty>()
					.Where(x =>
					       x != name
					       && x != folders
						   && x.Type != typeof(ProjectElementCollection)
						   && x.Type != typeof(FileMatchElementCollection)
						   )
					.Select(x => base[x])
					.Cast<CurrentConfigurationElementCollection>()
					.ToList();

				return commandConfigurations;
			}
		}

        public void AddCommandConfiguration(ConfigurationProperty commandConfiguration)
        {
            if (properties.Contains(commandConfiguration.Name)) return;
            properties.Add(commandConfiguration);
        }

        public T GetCommandConfiguration<T>(ConfigurationProperty commandConfiguration) where T : class
        {
            return (T) base[commandConfiguration];
        }

        public ConfigurationProperty GetConfigurationProperty(string configurationPropertyName)
        {
            return properties[configurationPropertyName];
        }

        /// <summary>
        /// Gets the collection of configuration properties contained by this configuration element.
        /// </summary>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection"></see> collection of properties for the element.</returns>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }
    }
}