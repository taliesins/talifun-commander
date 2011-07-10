using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public sealed class AudioConversionSettingElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty audioConversionType = new ConfigurationProperty("audioConversionType", typeof(AudioConversionType), AudioConversionType.NotSpecified, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioBitRate = new ConfigurationProperty("audioBitRate", typeof(int), 128000, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioFrequency = new ConfigurationProperty("audioFrequency", typeof(int), 44100, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioChannels = new ConfigurationProperty("audioChannels", typeof(int), 2, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="AudioConversionSettingElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AudioConversionSettingElement()
        {
            properties.Add(audioConversionType);
            properties.Add(audioBitRate);
            properties.Add(audioFrequency);
            properties.Add(audioChannels);
        }

        /// <summary>
        /// Gets or sets the conversion type to use.
        /// </summary>
        [ConfigurationProperty("audioConversionType", DefaultValue = AudioConversionType.NotSpecified, IsRequired = false)]
        public AudioConversionType AudioConversionType
        {
            get { return ((AudioConversionType)base[audioConversionType]); }
            set
            {
                if (value == AudioConversionType) return;

                base[audioConversionType] = value;
                OnPropertyChanged("AudioConversionType");
            }
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
        [ConfigurationProperty("audioBitRate", DefaultValue = 128000, IsRequired = false)]
        public int BitRate
        {
            get { return ((int)base[audioBitRate]); }
            set
            {
                if (value == BitRate) return;

                base[audioBitRate] = value;
                OnPropertyChanged("BitRate");
            }
        }

        /// <summary>
        /// Gets or sets the audio frequency to use for audio.
        /// Popular frequencies are:
        /// 96000
        /// 48000
        /// 44100
        /// 22050
        /// </summary>
        [ConfigurationProperty("audioFrequency", DefaultValue = 44100, IsRequired = false)]
        public int Frequency
        {
            get { return ((int)base[audioFrequency]); }
            set
            {
                if (value == Frequency) return;

                base[audioFrequency] = value;
                OnPropertyChanged("Frequency");
            }
        }

        /// <summary>
        /// Gets or sets the number of audio channels to use the audio.
        /// Popular values are
        /// 1 (Mono)
        /// 2 (Stero)
        /// </summary>
        [ConfigurationProperty("audioChannels", DefaultValue = 2, IsRequired = false)]
        public int Channel
        {
            get { return ((int)base[audioChannels]); }
            set
            {
                if (value == Channel) return;

                base[audioChannels] = value;
                OnPropertyChanged("Channel");
            }
        }
    }
}
