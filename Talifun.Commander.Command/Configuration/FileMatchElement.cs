using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the name of the configuration element represented by this instance.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue=null, IsRequired = true, IsKey=true)]
        public override string Name
        {
            get { return ((string)base[name]); }
            set { 
                if (value == Name) return;

                base[name] = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Regular expression to match file name against.
        /// </summary>
        /// <remarks>
        /// If this is left blank then it will be assumed that it matches.
        /// </remarks>
        [ConfigurationProperty("expression", DefaultValue = "", IsRequired = false)]
        public string Expression
        {
            get { return ((string)base[expression]); }
            set
            {
                if (value == Expression) return;

                base[expression] = value;
                OnPropertyChanged("Expression");
            }
        }

        /// <summary>
        /// The conversion type to use.
        /// </summary>
        [ConfigurationProperty("conversionType", DefaultValue = null, IsRequired = true)]
        public string ConversionType
        {
            get { return ((string)base[conversionType]); }
            set
            {
                if (value == ConversionType) return;

                base[conversionType] = value;
                OnPropertyChanged("ConversionType");
            }
        }

        /// <summary>
        /// The key name to use for the command settings.
        /// </summary>
        [ConfigurationProperty("commandSettingsKey", DefaultValue = null, IsRequired = true)]
        public string CommandSettingsKey
        {
            get { return ((string)base[commandSettingsKey]); }
            set
            {
                if (value == CommandSettingsKey) return;

                base[commandSettingsKey] = value;
                OnPropertyChanged("CommandSettingsKey");
            }
        }

        /// <summary>
        /// If a match is found then it should skip processing of all fileMatch elements, under this fileMatch, 
        /// in this fileMatches element.
        /// </summary>
        [ConfigurationProperty("stopProcessing", DefaultValue = false, IsRequired = false)]
        public bool StopProcessing
        {
            get { return ((bool)base[stopProcessing]); }
            set
            {
                if (value == StopProcessing) return;

                base[stopProcessing] = value;
                OnPropertyChanged("StopProcessing");
            }
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