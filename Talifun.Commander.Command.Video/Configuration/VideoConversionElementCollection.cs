using System.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Video.Configuration;

namespace Talifun.Commander.Command.Video
{
    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="VideoConversionSettingElement" /> configuration elements.
    /// </summary>
    [ConfigurationCollection(typeof(VideoConversionSettingElement))]
    public class VideoConversionSettingElementCollection : CurrentConfigurationElementCollection<VideoConversionSettingElement>
    {
        public VideoConversionSettingElementCollection()
        {
            Setting = VideoConversionConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
            return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(VideoConversionSettingElementCollection), null, ConfigurationPropertyOptions.None);
        }
    }
}
