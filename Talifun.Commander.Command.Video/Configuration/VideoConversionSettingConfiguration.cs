using System.Configuration;

namespace Talifun.Commander.Command.Video.Configuration
{
    public static class VideoConversionSettingConfiguration
    {
        public const string ConversionType = "VideoConversion";
        public const string CollectionSettingName = "videoConversionSettings";
        public const string ElementSettingName = "videoConversionSetting";

        public static string FFMpegPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FFMpegPath"];
            }
        }

        public static string FlvTool2Path
        {
            get
            {
                return ConfigurationManager.AppSettings["FlvTool2Path"];
            }
        }
    }
}
