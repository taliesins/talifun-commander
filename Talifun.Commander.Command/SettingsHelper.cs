using System.Configuration;

namespace Talifun.Commander.Command
{
    public static class SettingsHelper
    {
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

        public static string McAfeePath
        {
            get
            {
                return ConfigurationManager.AppSettings["McAfeePath"];
            }
        }

        public static string ConvertPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ConvertPath"];
            }
        }
    }
}
