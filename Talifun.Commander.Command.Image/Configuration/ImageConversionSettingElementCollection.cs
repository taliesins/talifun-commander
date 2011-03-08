using System.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Image.Configuration;

namespace Talifun.Commander.Command.Image
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="ImageConversionSettingElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(ImageConversionSettingElement))]
    public class ImageConversionSettingElementCollection : CurrentConfigurationElementCollection<ImageConversionSettingElement>
    {
        public ImageConversionSettingElementCollection()
        {
            CollectionSettingName = ImageConversionSettingConfiguration.CollectionSettingName;
            ElementSettingName = ImageConversionSettingConfiguration.ElementSettingName;
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(ImageConversionSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
