using System;
using System.Configuration;
using System.Drawing;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
    public class AntiVirusConfiguration : ISettingConfiguration
    {
        private AntiVirusConfiguration()
        {
        }

        public static readonly AntiVirusConfiguration Instance = new AntiVirusConfiguration();

        public string ConversionType
        {
            get { return "AntiVirus"; }
        }

        public string ElementCollectionSettingName
        {
            get { return "antiVirusSettings"; }
        }

        public string ElementSettingName
        {
            get { return "antiVirusSetting"; }
        }

    	public string McAfeePathSettingName
    	{
			get { return "McAfeePath"; }
    	}

        public string McAfeePath
        {
            get
            {
				return ConfigurationManager.AppSettings[McAfeePathSettingName];
            }
        }

        public Bitmap ElementImage
        {
            get { return Properties.Resource.AntiVirusElement; }
        }


        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.AntiVirusElementCollection; }
        }

        public Type ElementCollectionType
        {
            get { return typeof(AntiVirusElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (AntiVirusElement); }
        }
    }
}
