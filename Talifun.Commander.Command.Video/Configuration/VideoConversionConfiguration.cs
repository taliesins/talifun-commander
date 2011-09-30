using System;
using System.Configuration;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

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

    	public string FFMpegPathSettingName
    	{
			get { return "FFMpegPath"; }
    	}

        public string FFMpegPath
        {
            get
            {
				return ConfigurationManager.AppSettings[FFMpegPathSettingName];
            }
        }

    	public string FlvTool2PathSettingName
    	{
			get { return "FlvTool2Path"; }
    	}

        public string FlvTool2Path
        {
            get
            {
				return ConfigurationManager.AppSettings[FlvTool2PathSettingName];
            }
        }

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.VideoConversionElement.ToBitmapSource(); }
        }

        public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.VideoConversionElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof(VideoConversionElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof(VideoConversionElement); }
        }
    }
}
