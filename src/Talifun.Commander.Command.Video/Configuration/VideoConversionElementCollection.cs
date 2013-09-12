using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="VideoConversionElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(VideoConversionElement))]
    public class VideoConversionElementCollection : CurrentConfigurationElementCollection<VideoConversionElement>
    {
        public VideoConversionElementCollection()
        {
            Setting = VideoConversionConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(VideoConversionElementCollection), new VideoConversionElementCollection(), ConfigurationPropertyOptions.None);
        }
    }
}
