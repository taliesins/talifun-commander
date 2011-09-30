using System;
using System.Configuration;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
    public class VideoThumbnailerConfiguration : ISettingConfiguration
    {
        private VideoThumbnailerConfiguration()
        {
        }

        public static readonly VideoThumbnailerConfiguration Instance = new VideoThumbnailerConfiguration();

        public string ConversionType
        {
            get { return "VideoThumbnailer"; }
        }

        public string ElementCollectionSettingName
        {
            get { return "videoThumbnailerSettings"; }
        }

        public string ElementSettingName
        {
            get { return "videoThumbnailerSetting"; }
        }

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.VideoThumbnailerElement.ToBitmapSource(); }
        }

        public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.VideoThumbnailerElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof (VideoThumbnailerElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (VideoThumbnailerElement); }
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
    }
}
