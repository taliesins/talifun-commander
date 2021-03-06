﻿using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Talifun.Commander.Command.Configuration
{
	[JsonObject(MemberSerialization.OptIn)]
    public abstract class CommandConfigurationBase : NamedConfigurationElement, ICommandConfiguration
    {
        protected static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        protected static readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        protected static readonly ConfigurationProperty outPutPath = new ConfigurationProperty("outPutPath", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        protected static readonly ConfigurationProperty workingPath = new ConfigurationProperty("workingPath", typeof(string), null, ConfigurationPropertyOptions.None);
        protected static readonly ConfigurationProperty errorProcessingPath = new ConfigurationProperty("errorProcessingPath", typeof(string), null, ConfigurationPropertyOptions.None);
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
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public override string Name
        {
            get { return ((string)base[name]); }
			set { SetPropertyValue(value, name, "Name"); }
        }

        /// <summary>
        /// Gets or sets the path where processed file will be saved to.
        /// </summary>
        [ConfigurationProperty("outPutPath", DefaultValue = null, IsRequired = true)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OutPutPath
        {
            get { return ((string)base[outPutPath]); }
			set { SetPropertyValue(value, outPutPath, "OutPutPath"); }
        }

		public string GetOutPutPathOrDefault()
		{
			return string.IsNullOrEmpty(OutPutPath)
				? Configuration.CurrentConfiguration.DefaultPaths.OutPutPath(this)
				: OutPutPath;
		}

        /// <summary>
        /// Gets or sets the path where files will be saved to, while being processed. 
        /// </summary>
		/// <remarks>
		/// Excluding the configuration value from config will result in:
		/// the configuration defaults being used.
		/// 
		/// Setting an empty string for the configuration value will result in:
		/// the windows temp directory will be used to process files.
		/// </remarks>
        [ConfigurationProperty("workingPath", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string WorkingPath
        {
            get { return ((string)base[workingPath]); }
			set { SetPropertyValue(value, workingPath, "WorkingPath"); }
        }

		public string GetWorkingPathOrDefault()
		{
			return WorkingPath ?? Configuration.CurrentConfiguration.DefaultPaths.WorkingPath(this);
		}

        /// <summary>
        /// Gets or sets the path where files that could not be processed will be moved to.
        /// </summary>   
		/// <remarks>
		/// Excluding the configuration value from config will result in:
		/// the configuration defaults being used.
		/// 
		/// Setting an empty string for the configuration value will result in:
		/// the files that cannot be processed are just deleted
		/// </remarks>    
        [ConfigurationProperty("errorProcessingPath", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorProcessingPath
        {
            get { return ((string)base[errorProcessingPath]); }
			set { SetPropertyValue(value, errorProcessingPath, "ErrorProcessingPath"); }
        }

		public string GetErrorProcessingPathOrDefault()
		{
			return ErrorProcessingPath ?? Configuration.CurrentConfiguration.DefaultPaths.ErrorProcessingPath(this);
		}

        /// <summary>
        /// Gets or sets the file name format that is applied on outputted files.
        /// </summary>
        /// <remarks>This is the string.Format to apply to the outputted filename.</remarks>
        [ConfigurationProperty("fileNameFormat", DefaultValue = "")]
		[JsonProperty]
        public string FileNameFormat
        {
            get { return ((string)base[fileNameFormat]); }
			set { SetPropertyValue(value, fileNameFormat, "FileNameFormat"); }
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
