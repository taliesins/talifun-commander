using System.Configuration;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
    public class VideoThumbnailerSettingConfiguration : ISettingConfiguration
    {
        private VideoThumbnailerSettingConfiguration()
        {
        }

        public static readonly VideoThumbnailerSettingConfiguration Instance = new VideoThumbnailerSettingConfiguration();

        public string ConversionType
        {
            get { return "VideoThumbnailer"; }
        }

        public string CollectionSettingName
        {
            get { return "videoThumbnailerSettings"; }
        }

        public string ElementSettingName
        {
            get { return "videoThumbnailerSetting"; }
        }

        public string FFMpegPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FFMpegPath"];
            }
        }
    }
}
