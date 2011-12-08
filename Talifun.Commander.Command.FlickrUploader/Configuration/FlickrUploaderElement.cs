using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FlickrUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public sealed partial class FlickrUploaderElement : CommandConfigurationBase
	{
		private static readonly ConfigurationProperty flickrApiKey = new ConfigurationProperty("flickrApiKey", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty flickrApiSecret = new ConfigurationProperty("flickrApiSecret", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty flickrFrob = new ConfigurationProperty("flickrFrob", typeof(string), "", ConfigurationPropertyOptions.None);
		private static readonly ConfigurationProperty flickrAuthToken = new ConfigurationProperty("flickrAuthToken", typeof(string), "", ConfigurationPropertyOptions.None);

		/// <summary>
		/// Initializes the <see cref="FlickrUploaderElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static FlickrUploaderElement()
        {
			properties.Add(flickrApiKey);
			properties.Add(flickrApiSecret);
			properties.Add(flickrFrob);
			properties.Add(flickrAuthToken);
        }

		public FlickrUploaderElement()
		{
			Setting = FlickrUploaderConfiguration.Instance;	
		}

		/// <summary>
		/// Gets or sets the Flickr Api Key to be used when requesting a security token.
		/// </summary>       
		[ConfigurationProperty("flickrApiKey", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string FlickrApiKey
		{
			get { return ((string)base[flickrApiKey]); }
			set { SetPropertyValue(value, flickrApiKey, "FlickrApiKey"); }
		}

		/// <summary>
		/// Gets or sets the Flickr Api Secret to be used when requesting a security token.
		/// </summary>       
		[ConfigurationProperty("flickrApiSecret", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string FlickrApiSecret
		{
			get { return ((string)base[flickrApiSecret]); }
			set { SetPropertyValue(value, flickrApiSecret, "FlickrApiSecret"); }
		}

		/// <summary>
		/// Gets or sets the Flickr Frob to use when sending a request.
		/// </summary>       
		[ConfigurationProperty("flickrFrob", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string FlickrFrob
		{
			get { return ((string)base[flickrFrob]); }
			set { SetPropertyValue(value, flickrFrob, "FlickrFrob"); }
		}

		/// <summary>
		/// Gets or sets the Flickr Frob to use when sending a request.
		/// </summary>       
		[ConfigurationProperty("flickrAuthToken", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string FlickrAuthToken
		{
			get { return ((string)base[flickrAuthToken]); }
			set { SetPropertyValue(value, flickrAuthToken, "FlickrAuthToken"); }
		}
	}
}
