using System.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.VideoThumbNailer.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="VideoThumbnailerSettingElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(VideoThumbnailerSettingElement))]
    public class VideoThumbnailerSettingElementCollection : CurrentConfigurationElementCollection<VideoThumbnailerSettingElement>
    {
        public VideoThumbnailerSettingElementCollection()
        {
            CollectionSettingName = VideoThumbnailerSettingConfiguration.Instance.CollectionSettingName;
            ElementSettingName = VideoThumbnailerSettingConfiguration.Instance.ElementSettingName;
            AddElementName = ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(CollectionSettingName, typeof(VideoThumbnailerSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
