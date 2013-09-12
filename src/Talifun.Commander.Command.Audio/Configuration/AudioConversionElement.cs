using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
	[JsonObject(MemberSerialization.OptIn)]
    public sealed partial class AudioConversionElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty audioConversionType = new ConfigurationProperty("audioConversionType", typeof(AudioConversionType), AudioConversionType.NotSpecified, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioBitRate = new ConfigurationProperty("audioBitRate", typeof(int), 0, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioFrequency = new ConfigurationProperty("audioFrequency", typeof(int), 0, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioChannels = new ConfigurationProperty("audioChannels", typeof(int), 0, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="AudioConversionElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AudioConversionElement()
        {
            properties.Add(audioConversionType);
            properties.Add(audioBitRate);
            properties.Add(audioFrequency);
            properties.Add(audioChannels);
        }

		public AudioConversionElement()
		{
			Setting = AudioConversionConfiguration.Instance;	
		}

        /// <summary>
        /// Gets or sets the conversion type to use.
        /// </summary>
        [ConfigurationProperty("audioConversionType", DefaultValue = AudioConversionType.NotSpecified, IsRequired = false)]
		[JsonProperty]
        public AudioConversionType AudioConversionType
        {
            get { return ((AudioConversionType)base[audioConversionType]); }
			set { SetPropertyValue(value, audioConversionType, "AudioConversionType"); }
        }

        /// <summary>
        /// Gets or sets the bitrate to use for the audio.
        /// Popular rates are:
        /// 192000
        /// 128000
        /// 96000
        /// 64000
        /// 32000
        /// </summary>
        [ConfigurationProperty("audioBitRate", DefaultValue = 0, IsRequired = false)]
		[JsonProperty]
        public int BitRate
        {
            get { return ((int)base[audioBitRate]); }
			set { SetPropertyValue(value, audioBitRate, "BitRate"); }
        }

        /// <summary>
        /// Gets or sets the audio frequency to use for audio.
        /// Popular frequencies are:
        /// 96000
        /// 48000
        /// 44100
        /// 22050
        /// </summary>
        [ConfigurationProperty("audioFrequency", DefaultValue = 0, IsRequired = false)]
		[JsonProperty]
        public int Frequency
        {
            get { return ((int)base[audioFrequency]); }
			set { SetPropertyValue(value, audioFrequency, "Frequency"); }
        }

        /// <summary>
        /// Gets or sets the number of audio channels to use the audio.
        /// Popular values are
        /// 1 (Mono)
        /// 2 (Stero)
        /// </summary>
        [ConfigurationProperty("audioChannels", DefaultValue = 0, IsRequired = false)]
		[JsonProperty]
        public int Channel
        {
            get { return ((int)base[audioChannels]); }
			set { SetPropertyValue(value, audioChannels, "Channel"); }
        }
    }
}
