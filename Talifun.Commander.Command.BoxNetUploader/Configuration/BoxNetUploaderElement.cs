using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.BoxNetUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public sealed partial class BoxNetUploaderElement : CommandConfigurationBase
	{
		private static readonly ConfigurationProperty boxNetApiKey = new ConfigurationProperty("boxNetApiKey", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty boxNetUsername = new ConfigurationProperty("boxNetUsername", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty boxNetPassword = new ConfigurationProperty("boxNetPassword", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty boxNetFolder = new ConfigurationProperty("boxNetFolder", typeof(string), string.Empty, ConfigurationPropertyOptions.None);

		/// <summary>
		/// Initializes the <see cref="BoxNetUploaderElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static BoxNetUploaderElement()
        {
			properties.Add(boxNetApiKey);
			properties.Add(boxNetUsername);
			properties.Add(boxNetPassword);
			properties.Add(boxNetFolder);
		}

		public BoxNetUploaderElement()
		{
			Setting = BoxNetUploaderConfiguration.Instance;	
		}

		/// <summary>
		/// Gets or sets the Drop BoxNet Api Key to be used when authenticating.
		/// </summary>       
		[ConfigurationProperty("boxNetApiKey", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string BoxNetApiKey
		{
			get { return ((string)base[boxNetApiKey]); }
			set { SetPropertyValue(value, boxNetApiKey, "BoxNetApiKey"); }
		}

		/// <summary>
		/// Gets or sets the BoxNet username to use when authenticating.
		/// </summary>       
		[ConfigurationProperty("boxNetUsername", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string BoxNetUsername
		{
			get { return ((string)base[boxNetUsername]); }
			set { SetPropertyValue(value, boxNetUsername, "BoxNetUsername"); }
		}

		/// <summary>
		/// Gets or sets the Drop BoxNet password to use when authenticating.
		/// </summary>       
		[ConfigurationProperty("boxNetPassword", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string BoxNetPassword
		{
			get { return ((string)base[boxNetPassword]); }
			set { SetPropertyValue(value, boxNetPassword, "BoxNetPassword"); }
		}

		/// <summary>
		/// Gets or sets the Drop BoxNet Folder to use when uploading a file.
		/// </summary>       
		[ConfigurationProperty("boxNetFolder", DefaultValue = "")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string BoxNetFolder
		{
			get { return ((string)base[boxNetFolder]); }
			set { SetPropertyValue(value, boxNetFolder, "BoxNetFolder"); }
		}
	}
}
