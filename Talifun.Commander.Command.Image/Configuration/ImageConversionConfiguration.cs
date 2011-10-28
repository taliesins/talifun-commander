using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Image.Configuration
{
    public class ImageConversionConfiguration : ISettingConfiguration
    {
        private ImageConversionConfiguration()
        {
        }

        public static readonly ImageConversionConfiguration Instance = new ImageConversionConfiguration();

        public string ConversionType
        {
            get { return "ImageConversion"; }
        }

        public string ElementCollectionSettingName
        {
            get { return "imageConversionSettings"; }
        }

        public string ElementSettingName
        {
            get { return "imageConversionSetting"; }
        }

    	public string ConvertPathSettingName
    	{
			get { return "ConvertPath"; }
    	}

		public string CompositePathSettingName
		{
			get { return "CompositePath"; }
		}

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.ImageConversionElement.ToBitmapSource(); }
        }

        public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.ImageConversionElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof (ImageConversionElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (ImageConversionElement); }
        }
    }
}
