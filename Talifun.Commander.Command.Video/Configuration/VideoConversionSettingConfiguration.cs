using System.Configuration;
using System.Drawing;

namespace Talifun.Commander.Command.Video.Configuration
{
    public class VideoConversionSettingConfiguration : ISettingConfiguration
    {
        private VideoConversionSettingConfiguration()
        {
        }

        public static readonly VideoConversionSettingConfiguration Instance = new VideoConversionSettingConfiguration();

        public string ConversionType
        {
            get { return "VideoConversion"; }
        }

        public string CollectionSettingName
        {
            get { return "videoConversionSettings"; }
        }

        public string ElementSettingName
        {
            get { return "videoConversionSetting"; }
        }

        public string FFMpegPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FFMpegPath"];
            }
        }

        public string FlvTool2Path
        {
            get
            {
                return ConfigurationManager.AppSettings["FlvTool2Path"];
            }
        }

        public Image ElementImage
        {
            get { return Properties.Resource.videoconversionsetting; }
        }


        public Image ElementCollectionImage
        {
            get { return Properties.Resource.videoconversionsettings; }
        }
    }
}
