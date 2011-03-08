using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Talifun.Commander.Command.Configuration
{
    public sealed class CommanderSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty projects = new ConfigurationProperty("projects", typeof(ProjectElementCollection), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsDefaultCollection);

        /// <summary>
        /// Perform static initialisation for this configuration section. This includes explicitly adding
        /// configured properties to the Properties collection, and so cannot be performed inline.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static CommanderSection()
        {
            properties.Add(projects);
        }

        /// <summary>
        /// Gets a <see cref="ProjectElementCollection" /> containing the configuration elements associated with this configuration section.
        /// </summary>
        /// <value>A <see cref="ProjectElementCollection" /> containing the configuration elements associated with this configuration section.</value>
        [ConfigurationProperty("projects", DefaultValue = null, IsRequired = true, IsDefaultCollection = true)]
        public ProjectElementCollection Projects
        {
            get { return ((ProjectElementCollection)base[projects]); }
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