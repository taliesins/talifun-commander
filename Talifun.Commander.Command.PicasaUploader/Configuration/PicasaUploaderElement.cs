using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.PicasaUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public sealed partial class PicasaUploaderElement : CommandConfigurationBase
	{
		private static readonly ConfigurationProperty googleUsername = new ConfigurationProperty("googleUsername", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty googlePassword = new ConfigurationProperty("googlePassword", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty picasaUsername = new ConfigurationProperty("picasaUsername", typeof(string), "default", ConfigurationPropertyOptions.None);
		private static readonly ConfigurationProperty applicationName = new ConfigurationProperty("applicationName", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
		private static readonly ConfigurationProperty albumId = new ConfigurationProperty("albumId", typeof(string), "default", ConfigurationPropertyOptions.None);

		/// <summary>
		/// Initializes the <see cref="PicasaUploaderElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static PicasaUploaderElement()
        {
			properties.Add(googleUsername);
			properties.Add(googlePassword);
			properties.Add(picasaUsername);
			properties.Add(applicationName);
			properties.Add(albumId);
        }

		public PicasaUploaderElement()
		{
			Setting = PicasaUploaderConfiguration.Instance;	
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
		/// Gets or sets the Picasa Username to use when sending a request.
		/// </summary>       
		[ConfigurationProperty("picasaUsername", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string PicasaUsername
		{
			get { return ((string)base[picasaUsername]); }
			set { SetPropertyValue(value, picasaUsername, "PicasaUsername"); }
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

		/// <summary>
		/// Gets or sets the album to use when sending a request.
		/// </summary>       
		[ConfigurationProperty("albumId", DefaultValue = null)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string AlbumId
		{
			get { return ((string)base[albumId]); }
			set { SetPropertyValue(value, albumId, "AlbumId"); }
		}
	}
}
