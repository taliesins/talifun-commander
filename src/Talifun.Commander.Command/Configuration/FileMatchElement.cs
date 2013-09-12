using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public sealed partial class FileMatchElement : NamedConfigurationElement
    {
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty expression = new ConfigurationProperty("expression", typeof(string), "", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty conversionType = new ConfigurationProperty("conversionType", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty commandSettingsKey = new ConfigurationProperty("commandSettingsKey", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty stopProcessing = new ConfigurationProperty("stopProcessing", typeof(bool), false, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="FileMatchElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static FileMatchElement()
        {
            properties.Add(name);
            properties.Add(expression);
            properties.Add(conversionType);
            properties.Add(commandSettingsKey);
            properties.Add(stopProcessing);
        }

		public FileMatchElement()
		{
			Setting = FileMatchConfiguration.Instance;	
		}

        /// <summary>
        /// Gets or sets the name of the configuration element represented by this instance.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue=null, IsRequired = true, IsKey=true)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public override string Name
        {
            get { return ((string)base[name]); }
			set { SetPropertyValue(value, name, "Name"); }
        }

        /// <summary>
        /// Regular expression to match file name against.
        /// </summary>
        /// <remarks>
        /// If this is left blank then it will be assumed that it matches.
        /// </remarks>
        [ConfigurationProperty("expression", DefaultValue = "", IsRequired = false)]
		[JsonProperty]
        public string Expression
        {
            get { return ((string)base[expression]); }
			set { SetPropertyValue(value, expression, "Expression"); }
        }

        /// <summary>
        /// The conversion type to use.
        /// </summary>
        [ConfigurationProperty("conversionType", DefaultValue = null, IsRequired = true)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConversionType
        {
            get { return ((string)base[conversionType]); }
			set { SetPropertyValue(value, conversionType, "ConversionType"); }
        }

        /// <summary>
        /// The key name to use for the command settings.
        /// </summary>
        [ConfigurationProperty("commandSettingsKey", DefaultValue = null, IsRequired = true)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CommandSettingsKey
        {
            get { return ((string)base[commandSettingsKey]); }
			set { SetPropertyValue(value, commandSettingsKey, "CommandSettingsKey"); }
        }

        /// <summary>
        /// If a match is found then it should skip processing of all fileMatch elements, under this fileMatch, 
        /// in this fileMatches element.
        /// </summary>
        [ConfigurationProperty("stopProcessing", DefaultValue = false, IsRequired = false)]
		[JsonProperty]
        public bool StopProcessing
        {
            get { return ((bool)base[stopProcessing]); }
			set { SetPropertyValue(value, stopProcessing, "StopProcessing"); }
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