using System.Configuration;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="AudioConversionSettingElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(AudioConversionSettingElement))]
    public class AudioConversionSettingElementCollection : CurrentConfigurationElementCollection<AudioConversionSettingElement>
    {
        public AudioConversionSettingElementCollection()
        {
            CollectionSettingName = AudioConversionSettingConfiguration.Instance.CollectionSettingName;
            ElementSettingName = AudioConversionSettingConfiguration.Instance.ElementSettingName;
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(AudioConversionSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
