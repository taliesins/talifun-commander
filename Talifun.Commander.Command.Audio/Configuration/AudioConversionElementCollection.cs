using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="AudioConversionElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(AudioConversionElement))]
    public class AudioConversionElementCollection : CurrentConfigurationElementCollection<AudioConversionElement>
    {
        public AudioConversionElementCollection()
        {
            Setting = AudioConversionConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(AudioConversionElementCollection), new AudioConversionElementCollection(), ConfigurationPropertyOptions.None);
        }
    }
}
