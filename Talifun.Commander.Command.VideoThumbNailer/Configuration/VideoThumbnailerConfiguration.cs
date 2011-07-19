using System;
using System.Configuration;
using System.Drawing;

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

        public Bitmap ElementImage
        {
            get { return Properties.Resource.VideoThumbnailerElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.VideoThumbnailerElementCollection; }
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
