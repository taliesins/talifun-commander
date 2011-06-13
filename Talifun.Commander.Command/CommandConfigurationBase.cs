using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    public abstract class CommandConfigurationBase : NamedConfigurationElement, ICommandConfiguration
    {
        protected static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        protected static readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        protected static readonly ConfigurationProperty outPutPath = new ConfigurationProperty("outPutPath", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        protected static readonly ConfigurationProperty workingPath = new ConfigurationProperty("workingPath", typeof(string), "", ConfigurationPropertyOptions.None);
        protected static readonly ConfigurationProperty errorProcessingPath = new ConfigurationProperty("errorProcessingPath", typeof(string), "", ConfigurationPropertyOptions.None);
        protected static readonly ConfigurationProperty fileNameFormat = new ConfigurationProperty("fileNameFormat", typeof(string), "", ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="CommandConfigurationBase"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static CommandConfigurationBase()
        {
            properties.Add(name);
            properties.Add(outPutPath);
            properties.Add(workingPath);
            properties.Add(errorProcessingPath);
            properties.Add(fileNameFormat);
        }

        /// <summary>
        /// Gets or sets the name of the configuration element represented by this instance.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = null, IsRequired = true, IsKey = true)]
        public override string Name
        {
            get { return ((string)base[name]); }
            set { base[name] = value; }
        }

        /// <summary>
        /// Gets or sets the path where processed file will be saved to.
        /// </summary>
        [ConfigurationProperty("outPutPath", DefaultValue = null, IsRequired = true)]
        public string OutPutPath
        {
            get { return ((string)base[outPutPath]); }
            set { base[outPutPath] = value; }
        }

        /// <summary>
        /// Gets or sets the path where files will be saved to, while being processed. 
        /// </summary>
        /// <remarks>
        /// Basically the temp directory for processing
        /// </remarks>
        [ConfigurationProperty("workingPath", DefaultValue = "")]
        public string WorkingPath
        {
            get { return ((string)base[workingPath]); }
            set { base[workingPath] = value; }
        }

        /// <summary>
        /// Gets or sets the path where files that could not be processed will be moved to.
        /// </summary>       
        [ConfigurationProperty("errorProcessingPath", DefaultValue = "")]
        public string ErrorProcessingPath
        {
            get { return ((string)base[errorProcessingPath]); }
            set { base[errorProcessingPath] = value; }
        }

        /// <summary>
        /// Gets or sets the file name format that is applied on outputted files.
        /// </summary>
        [ConfigurationProperty("fileNameFormat", DefaultValue = "")]
        public string FileNameFormat
        {
            get { return ((string)base[fileNameFormat]); }
            set { base[fileNameFormat] = value; }
        }

        /// <summary>
        /// Gets the collection of configuration properties contained by this configuration element.
        /// </summary>
        public new ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }
    }
}
