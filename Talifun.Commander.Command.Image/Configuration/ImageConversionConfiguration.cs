using System;
using System.Configuration;
using System.Drawing;

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

        public string ConvertPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ConvertPath"];
            }
        }

        public Bitmap ElementImage
        {
            get { return Properties.Resource.ImageConversionElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.ImageConversionElementCollection; }
        }

        public Type ElementCollectionType
        {
            get { return typeof (ImageConversionSettingElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (ImageConversionSettingElement); }
        }
    }
}
