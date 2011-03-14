using System.Configuration;

namespace Talifun.Commander.Command.Image.Configuration
{
    public class ImageConversionSettingConfiguration : ISettingConfiguration
    {
        private ImageConversionSettingConfiguration()
        {
        }

        public static readonly ImageConversionSettingConfiguration Instance = new ImageConversionSettingConfiguration();

        public string ConversionType
        {
            get { return "ImageConversion"; }
        }

        public string CollectionSettingName
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
    }
}
