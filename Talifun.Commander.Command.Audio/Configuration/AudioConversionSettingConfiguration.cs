using System.Configuration;

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
    }
}
