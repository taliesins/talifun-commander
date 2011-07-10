using System;
using System.Configuration;
using System.Drawing;

namespace Talifun.Commander.Command.Video.Configuration
{
    public class VideoConversionConfiguration : ISettingConfiguration
    {
        private VideoConversionConfiguration()
        {
        }

        public static readonly VideoConversionConfiguration Instance = new VideoConversionConfiguration();

        public string ConversionType
        {
            get { return "VideoConversion"; }
        }

        public string ElementCollectionSettingName
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

        public Bitmap ElementImage
        {
            get { return Properties.Resource.VideoConversionElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.VideoConversionElementCollection; }
        }

        public Type ElementCollectionType
        {
            get { return typeof(VideoConversionSettingElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof(VideoConversionSettingElement); }
        }
    }
}
