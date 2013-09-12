using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ProjectElement : NamedConfigurationElement
    {
		private static readonly string[] _excludedElements = new[] { "project", "folder", "fileMatch" };

		protected static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        protected static readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        protected static readonly ConfigurationProperty folders = new ConfigurationProperty("folders", typeof(FolderElementCollection), new FolderElementCollection(), ConfigurationPropertyOptions.None);
        
		/// <summary>
        /// Initializes the <see cref="FileMatchElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static ProjectElement()
        {
			properties.Add(name);
			properties.Add(folders);

			var container = CommandContainer.Instance.Container;

			var elementCollectionConfigurationProperties = container.GetExportedValues<CurrentConfigurationElementCollection>()
				.Where(x => !_excludedElements.Contains(x.Setting.ElementSettingName));

			foreach (var elementCollectionConfigurationProperty in elementCollectionConfigurationProperties)
			{
				properties.Add(elementCollectionConfigurationProperty.CreateNewConfigurationProperty());
			}
        }

        /// <summary>
        /// Initializes the <see cref="ProjectElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        public ProjectElement()
        {
			Setting = ProjectConfiguration.Instance;
        }

        /// <summary>
        /// Gets or sets the name of the configuration element represented by this instance.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = null, IsRequired = true, IsKey = true)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
		[JsonProperty]
        public FolderElementCollection Folders
        {
            get { return ((FolderElementCollection)base[folders]); }
			set { SetPropertyValue(value, folders, "Folders"); }
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

		[JsonProperty(TypeNameHandling = TypeNameHandling.All)]
		private ExpandoObject CommandPluginProperties
		{
			get
			{
				var commandConfigurations = properties.Cast<ConfigurationProperty>()
					.Where(x =>
					       x != name
					       && x != folders
					       && x.Type != typeof (ProjectElementCollection)
					       && x.Type != typeof (FileMatchElementCollection)
					);

				var commandPluginProperties = new ExpandoObject();
				foreach (var configurationProperty in commandConfigurations)
				{
					((IDictionary<string, object>)commandPluginProperties).Add(configurationProperty.Name, base[configurationProperty]);
				}
				
				return commandPluginProperties;
			}
			set
			{
                foreach (var commandPluginProperty in value.Where(x => x.Value is ISettingConfiguration))
				{
					base[commandPluginProperty.Key] = commandPluginProperty.Value;
				}
			}
		}

		public T GetElement<T>(FileMatchElement fileMatch, string elementCollectionSettingName) where T : NamedConfigurationElement
		{
			var commandElementCollection = (CurrentConfigurationElementCollection)base[elementCollectionSettingName];
			var commandElement = commandElementCollection[fileMatch.CommandSettingsKey];

			if (commandElement == null)
			{
				throw new ConfigurationErrorsException(string.Format(Resource.ErrorMessageFileMatchNoMatchingConversionSettingsKey,
				                                                     fileMatch.CommandSettingsKey, elementCollectionSettingName));
			}

			return (T)commandElement;
		}

        public void AddElementCollection(ConfigurationProperty elementCollectionConfigurationProperty)
        {
            if (properties.Contains(elementCollectionConfigurationProperty.Name)) return;
            properties.Add(elementCollectionConfigurationProperty);
        }

		public T GetElementCollection<T>(ConfigurationProperty commandConfiguration) where T : CurrentConfigurationElementCollection
        {
            return (T) base[commandConfiguration];
        }

		public T GetElementCollection<T>(string elementCollectionSettingName) where T : CurrentConfigurationElementCollection
		{
			return (T)base[elementCollectionSettingName];
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