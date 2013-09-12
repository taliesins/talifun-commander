using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Image.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="ImageConversionElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(ImageConversionElement))]
    public class ImageConversionElementCollection : CurrentConfigurationElementCollection<ImageConversionElement>
    {
        public ImageConversionElementCollection()
        {
            Setting = ImageConversionConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(ImageConversionElementCollection), new ImageConversionElementCollection(), ConfigurationPropertyOptions.None);
        }
    }
}
