using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public sealed partial class YouTubeUploaderElement : CommandConfigurationBase
	{
		private static readonly ConfigurationProperty googleUsername = new ConfigurationProperty("googleUsername", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty googlePassword = new ConfigurationProperty("googlePassword", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty youTubeUsername = new ConfigurationProperty("youTubeUsername", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty developerKey = new ConfigurationProperty("developerKey", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty applicationName = new ConfigurationProperty("applicationName", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		/// <summary>
		/// Initializes the <see cref="YouTubeUploaderElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static YouTubeUploaderElement()
        {
			properties.Add(googleUsername);
			properties.Add(googlePassword);
			properties.Add(youTubeUsername);
			properties.Add(developerKey);
			properties.Add(applicationName);
        }

		public YouTubeUploaderElement()
		{
			Setting = YouTubeUploaderConfiguration.Instance;	
		}

		/// <summary>
		/// Gets or sets the Google Username to be used when requesting a security token.
		/// </summary>       
		[ConfigurationProperty("googleUsername", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string GoogleUsername
		{
			get { return ((string)base[googleUsername]); }
			set { SetPropertyValue(value, googleUsername, "GoogleUsername"); }
		}

		/// <summary>
		/// Gets or sets the Google Password to be used when requesting a security token.
		/// </summary>       
		[ConfigurationProperty("googlePassword", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string GooglePassword
		{
			get { return ((string)base[googlePassword]); }
			set { SetPropertyValue(value, googlePassword, "GooglePassword"); }
		}

		/// <summary>
		/// Gets or sets the YouTube Username to use when sending a request.
		/// </summary>       
		[ConfigurationProperty("youTubeUsername", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string YouTubeUsername
		{
			get { return ((string)base[youTubeUsername]); }
			set { SetPropertyValue(value, youTubeUsername, "YouTubeUsername"); }
		}

		/// <summary>
		/// Gets or sets the developer key to use when sending a request.
		/// </summary>       
		[ConfigurationProperty("developerKey", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DeveloperKey
		{
			get { return ((string)base[developerKey]); }
			set { SetPropertyValue(value, developerKey, "DeveloperKey"); }
		}

		/// <summary>
		/// Gets or sets the application name to use when sending a request.
		/// </summary>       
		[ConfigurationProperty("applicationName", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ApplicationName
		{
			get { return ((string)base[applicationName]); }
			set { SetPropertyValue(value, applicationName, "ApplicationName"); }
		}
	}
}
