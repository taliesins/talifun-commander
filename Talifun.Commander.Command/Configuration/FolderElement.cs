using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public sealed partial class FolderElement : NamedConfigurationElement
    {
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty folderToWatch = new ConfigurationProperty("folderToWatch", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty fileNameFilter = new ConfigurationProperty("fileNameFilter", typeof(string), "", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty pollTime = new ConfigurationProperty("pollTime", typeof(int), 3000, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty includeSubdirectories = new ConfigurationProperty("includeSubdirectories", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty workingPath = new ConfigurationProperty("workingPath", typeof(string), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty completedPath = new ConfigurationProperty("completedPath", typeof(string), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty fileMatches = new ConfigurationProperty("fileMatches", typeof(FileMatchElementCollection), new FileMatchElementCollection(), ConfigurationPropertyOptions.None | ConfigurationPropertyOptions.IsDefaultCollection);

        /// <summary>
        /// Initializes the <see cref="FolderElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static FolderElement()
        {
            properties.Add(name);
            properties.Add(folderToWatch);
            properties.Add(fileNameFilter);
            properties.Add(pollTime);
            properties.Add(includeSubdirectories);
            properties.Add(workingPath);
            properties.Add(completedPath);
            properties.Add(fileMatches);
        }

		public FolderElement()
		{
			Setting = FolderConfiguration.Instance;	
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
        /// The folder to watch for file events.
        /// </summary>
        /// <remarks>
        /// There should only be one watcher per a folder branch.
        /// </remarks>
        [ConfigurationProperty("folderToWatch", DefaultValue=null, IsRequired = true)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FolderToWatch
        {
            get { return ((string)base[folderToWatch]); }
			set { SetPropertyValue(value, folderToWatch, "FolderToWatch"); }
        }

		public string GetFolderToWatchOrDefault()
		{
			return String.IsNullOrEmpty(FolderToWatch)
				? Configuration.CurrentConfiguration.DefaultPaths.FolderToWatch(this)
				: FolderToWatch;
		}

        /// <summary>
        /// The wildcard filter to use for files. E.g. *.txt
        /// </summary>
        [ConfigurationProperty("fileNameFilter", DefaultValue = "")]
		[JsonProperty]
        public string Filter
        {
            get { return ((string)base[fileNameFilter]); }
			set { SetPropertyValue(value, fileNameFilter, "Filter"); }
        }

        /// <summary>
        /// The amount of time to wait, in milliseconds, without file activity before assuming that changes are complete.
        /// </summary>
        [ConfigurationProperty("pollTime", DefaultValue=3000)]
		[JsonProperty]
        public int PollTime
        {
            get { return ((int)base[pollTime]); }
			set { SetPropertyValue(value, pollTime, "PollTime"); }
        }

        /// <summary>
        /// The amount of time to wait without file activity before assuming that changes are complete.
        /// </summary>
        /// <remarks>
        /// When setting this to true please ensure that there are no overlapping FolderElements. Otherwise
        /// unexpected results may occur.
        /// </remarks>
        [ConfigurationProperty("includeSubdirectories", DefaultValue = false)]
		[JsonProperty]
        public bool IncludeSubdirectories
        {
            get { return ((bool)base[includeSubdirectories]); }
			set { SetPropertyValue(value, includeSubdirectories, "IncludeSubdirectories"); }
        }

        /// <summary>
        /// Gets or sets the path where file will be moved before being processed.
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
        /// Gets or sets the path where file will be moved after it is finished being processed.
        /// </summary>
        /// <remarks>
        /// Excluding the configuration value from config will result in:
        /// the configuration defaults being used.
        /// 
		/// Setting an empty string for the configuration value will result in:
		/// not moving files to the completed path once they have been successfully processed.
        /// </remarks>
        [ConfigurationProperty("completedPath", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompletedPath
        {
            get { return ((string)base[completedPath]); }
			set { SetPropertyValue(value, completedPath, "CompletedPath"); }
        }

		public string GetCompletedPathOrDefault()
		{
			return CompletedPath ?? Configuration.CurrentConfiguration.DefaultPaths.CompletedPath(this);
		}

        /// <summary>
        /// Gets a <see cref="FileMatchElementCollection" /> containing the <see cref="FileMatchElement" /> elements
        /// for the conversion type to run upon matching.
        /// </summary>
        /// <value>A <see cref="ProviderSettingsCollection" /> containing the configuration elements associated with this configuration section.</value>
        [ConfigurationProperty("fileMatches", DefaultValue = null, IsDefaultCollection = true)]
		[JsonProperty]
        public FileMatchElementCollection FileMatches
        {
            get { return ((FileMatchElementCollection)base[fileMatches]); }
			set { SetPropertyValue(value, fileMatches, "FileMatches"); }
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