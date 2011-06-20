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
            Setting = VideoThumbnailerSettingConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.CollectionSettingName, typeof(VideoThumbnailerSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
