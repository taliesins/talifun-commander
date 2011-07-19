using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="VideoThumbnailerElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(VideoThumbnailerElement))]
    public class VideoThumbnailerElementCollection : CurrentConfigurationElementCollection<VideoThumbnailerElement>
    {
        public VideoThumbnailerElementCollection()
        {
            Setting = VideoThumbnailerConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(VideoThumbnailerElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
