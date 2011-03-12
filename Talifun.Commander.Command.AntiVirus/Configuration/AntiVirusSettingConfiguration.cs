using System.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    public static class AntiVirusSettingConfiguration
    {
        public const string ConversionType = "AntiVirus";
        public const string CollectionSettingName = "antiVirusSettings";
        public const string ElementSettingName = "antiVirusSetting";

        public static string McAfeePath
        {
            get
            {
                return ConfigurationManager.AppSettings["McAfeePath"];
            }
        }
    }
}
