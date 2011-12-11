using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public sealed partial class DropBoxUploaderElement : CommandConfigurationBase
	{
		private static readonly ConfigurationProperty dropBoxApiKey = new ConfigurationProperty("dropBoxApiKey", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty dropBoxApiSecret = new ConfigurationProperty("dropBoxApiSecret", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty dropBoxRequestKey = new ConfigurationProperty("dropBoxRequestKey", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty dropBoxRequestSecret = new ConfigurationProperty("dropBoxRequestSecret", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty dropBoxAuthenticationKey = new ConfigurationProperty("dropBoxAuthenticationKey", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty dropBoxAuthenticationSecret = new ConfigurationProperty("dropBoxAuthenticationSecret", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty dropBoxFolder = new ConfigurationProperty("dropBoxFolder", typeof(string), string.Empty, ConfigurationPropertyOptions.None);


		/// <summary>
		/// Initializes the <see cref="DropBoxUploaderElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static DropBoxUploaderElement()
        {
			properties.Add(dropBoxApiKey);
			properties.Add(dropBoxApiSecret);
			properties.Add(dropBoxRequestKey);
			properties.Add(dropBoxRequestSecret);
			properties.Add(dropBoxAuthenticationKey);
			properties.Add(dropBoxAuthenticationSecret);
			properties.Add(dropBoxFolder);
		}

		public DropBoxUploaderElement()
		{
			Setting = DropBoxUploaderConfiguration.Instance;	
		}

		/// <summary>
		/// Gets or sets the Drop Box Api Key to be used when requesting a request token.
		/// </summary>       
		[ConfigurationProperty("dropBoxApiKey", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DropBoxApiKey
		{
			get { return ((string)base[dropBoxApiKey]); }
			set { SetPropertyValue(value, dropBoxApiKey, "DropBoxApiKey"); }
		}

		/// <summary>
		/// Gets or sets the Drop Box Api Secret to be used when requesting a request token.
		/// </summary>       
		[ConfigurationProperty("dropBoxApiSecret", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DropBoxApiSecret
		{
			get { return ((string)base[dropBoxApiSecret]); }
			set { SetPropertyValue(value, dropBoxApiSecret, "DropBoxApiSecret"); }
		}

		/// <summary>
		/// Gets or sets the DropBox request key to use when requesting an authentication token.
		/// </summary>       
		[ConfigurationProperty("dropBoxRequestKey", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DropBoxRequestKey
		{
			get { return ((string)base[dropBoxRequestKey]); }
			set { SetPropertyValue(value, dropBoxRequestKey, "DropBoxRequestKey"); }
		}

		/// <summary>
		/// Gets or sets the Drop Box request secret to use when requesting an authentication token.
		/// </summary>       
		[ConfigurationProperty("dropBoxRequestSecret", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DropBoxRequestSecret
		{
			get { return ((string)base[dropBoxRequestSecret]); }
			set { SetPropertyValue(value, dropBoxRequestSecret, "DropBoxRequestSecret"); }
		}

		/// <summary>
		/// Gets or sets the DropBox request key to use when requesting an authentication token.
		/// </summary>       
		[ConfigurationProperty("dropBoxAuthenticationKey", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DropBoxAuthenticationKey
		{
			get { return ((string)base[dropBoxAuthenticationKey]); }
			set { SetPropertyValue(value, dropBoxAuthenticationKey, "DropBoxAuthenticationKey"); }
		}

		/// <summary>
		/// Gets or sets the Drop Box request secret to use when requesting an authentication token.
		/// </summary>       
		[ConfigurationProperty("dropBoxAuthenticationSecret", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DropBoxAuthenticationSecret
		{
			get { return ((string)base[dropBoxAuthenticationSecret]); }
			set { SetPropertyValue(value, dropBoxAuthenticationSecret, "DropBoxAuthenticationSecret"); }
		}

		/// <summary>
		/// Gets or sets the Drop Box Folder to use when uploading a file.
		/// </summary>       
		[ConfigurationProperty("dropBoxFolder", DefaultValue = "")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string DropBoxFolder
		{
			get { return ((string)base[dropBoxFolder]); }
			set { SetPropertyValue(value, dropBoxFolder, "DropBoxFolder"); }
		}
	}
}
