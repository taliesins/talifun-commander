using System.Configuration;
using System.Drawing;

namespace Talifun.Commander.Command.Audio.Configuration
{
    public class AudioConversionSettingConfiguration : ISettingConfiguration
    {
        private AudioConversionSettingConfiguration()
        {
        }

        public static readonly AudioConversionSettingConfiguration Instance = new AudioConversionSettingConfiguration();

        public string ConversionType
        {
            get { return "AudioConversion"; }
        }

        public string CollectionSettingName
        {
            get { return "audioConversionSettings"; }
        }

        public string ElementSettingName
        {
            get { return "audioConversionSetting"; }
        }

        public string FFMpegPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FFMpegPath"];
            }
        }

        public Image ElementImage
        {
            get { return Properties.Resource.audioconversionsetting; }
        }


        public Image ElementCollectionImage
        {
            get { return Properties.Resource.audioconversionsettings; }
        }
    }
}
