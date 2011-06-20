using System.Configuration;
using System.Drawing;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    public class AntiVirusSettingConfiguration : ISettingConfiguration
    {
        private AntiVirusSettingConfiguration()
        {
        }

        public static readonly AntiVirusSettingConfiguration Instance = new AntiVirusSettingConfiguration();

        public string ConversionType
        {
            get { return "AntiVirus"; }
        }

        public string CollectionSettingName
        {
            get { return "antiVirusSettings"; }
        }

        public string ElementSettingName
        {
            get { return "antiVirusSetting"; }
        }

        public string McAfeePath
        {
            get
            {
                return ConfigurationManager.AppSettings["McAfeePath"];
            }
        }

        public Image ElementImage
        {
            get { return Properties.Resource.antiVirusSetting; }
        }


        public Image ElementCollectionImage
        {
            get { return Properties.Resource.antivirussettings; }
        }
    }
}
