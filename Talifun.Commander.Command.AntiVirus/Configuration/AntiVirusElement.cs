using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public sealed class AntiVirusSettingElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty virusScannerType = new ConfigurationProperty("virusScannerType", typeof(VirusScannerType), VirusScannerType.NotSpecified, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="AntiVirusSettingElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AntiVirusSettingElement()
        {
            properties.Add(virusScannerType);
        }

        /// <summary>
        /// Gets or sets the virus scanner to use.
        /// </summary>
        [ConfigurationProperty("virusScannerType", DefaultValue = VirusScannerType.NotSpecified, IsRequired = false)]
        public VirusScannerType VirusScannerType
        {
            get { return ((VirusScannerType)base[virusScannerType]); }
            set
            {
                if (value == VirusScannerType) return;

                base[virusScannerType] = value;
                OnPropertyChanged("VirusScannerType");
            }
        }
    }
}