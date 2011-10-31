using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public sealed partial class FolderElement : NamedConfigurationElement, IDataErrorInfo
    {
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty folderToWatch = new ConfigurationProperty("folderToWatch", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty fileNameFilter = new ConfigurationProperty("fileNameFilter", typeof(string), "", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty pollTime = new ConfigurationProperty("pollTime", typeof(int), 3000, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty includeSubdirectories = new ConfigurationProperty("includeSubdirectories", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty workingPath = new ConfigurationProperty("workingPath", typeof(string), "", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty completedPath = new ConfigurationProperty("completedPath", typeof(string), "", ConfigurationPropertyOptions.None);
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
        public string FolderToWatch
        {
            get { return ((string)base[folderToWatch]); }
			set { SetPropertyValue(value, folderToWatch, "FolderToWatch"); }
        }

		public string GetFolderToWatchOrDefault()
		{
			return string.IsNullOrEmpty(FolderToWatch)
				? Configuration.CurrentConfiguration.DefaultPaths.FolderToWatch(this)
				: FolderToWatch;
		}

        /// <summary>
        /// The wildcard filter to use for files. E.g. *.txt
        /// </summary>
        [ConfigurationProperty("fileNameFilter", DefaultValue = "")]
        public string Filter
        {
            get { return ((string)base[fileNameFilter]); }
			set { SetPropertyValue(value, fileNameFilter, "Filter"); }
        }

        /// <summary>
        /// The amount of time to wait, in milliseconds, without file activity before assuming that changes are complete.
        /// </summary>
        [ConfigurationProperty("pollTime", DefaultValue=3000)]
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
        public bool IncludeSubdirectories
        {
            get { return ((bool)base[includeSubdirectories]); }
			set { SetPropertyValue(value, includeSubdirectories, "IncludeSubdirectories"); }
        }

        /// <summary>
        /// Gets or sets the path where file will be moved before being processed.
        /// </summary>
        /// <remarks>
        /// Leaving this blank will result in the file staying where it is. This means that other programs 
        /// may still potentially modify the file (ftp upload for instance) and could result in unexpected 
        /// results if there are multiple fileMatch elements. So to make it more "transactional" we move it
        /// to another location that other programs will not know about.
        /// </remarks>
        [ConfigurationProperty("workingPath", DefaultValue = "")]
        public string WorkingPath
        {
            get { return ((string)base[workingPath]); }
			set { SetPropertyValue(value, workingPath, "WorkingPath"); }
        }

		public string GetWorkingPathOrDefault()
		{
			return string.IsNullOrEmpty(WorkingPath)
				? Configuration.CurrentConfiguration.DefaultPaths.WorkingPath(this)
				: WorkingPath;
		}

    	/// <summary>
        /// Gets or sets the path where file will be moved after it is finished being processed.
        /// </summary>
        /// <remarks>
        /// Leaving this blank will result in the file being deleted, once it has been processed by all
        /// matching FileMatch elements.
        /// </remarks>
        [ConfigurationProperty("completedPath", DefaultValue = "")]
        public string CompletedPath
        {
            get { return ((string)base[completedPath]); }
			set { SetPropertyValue(value, completedPath, "CompletedPath"); }
        }

		public string GetCompletedPathOrDefault()
		{
			return string.IsNullOrEmpty(CompletedPath)
				? Configuration.CurrentConfiguration.DefaultPaths.CompletedPath(this)
				: CompletedPath;
		}

        /// <summary>
        /// Gets a <see cref="FileMatchElementCollection" /> containing the <see cref="FileMatchElement" /> elements
        /// for the conversion type to run upon matching.
        /// </summary>
        /// <value>A <see cref="ProviderSettingsCollection" /> containing the configuration elements associated with this configuration section.</value>
        [ConfigurationProperty("fileMatches", DefaultValue = null, IsDefaultCollection = true)]
        public FileMatchElementCollection FileMatches
        {
            get { return ((FileMatchElementCollection)base[fileMatches]); }
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