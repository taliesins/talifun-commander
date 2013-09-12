using System;
using System.Configuration;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Audio.Configuration
{
    public class AudioConversionConfiguration : ISettingConfiguration
    {
        private AudioConversionConfiguration()
        {
        }

        public static readonly AudioConversionConfiguration Instance = new AudioConversionConfiguration();

        public string ConversionType
        {
            get { return "AudioConversion"; }
        }

        public string ElementCollectionSettingName
        {
            get { return "audioConversionSettings"; }
        }

        public string ElementSettingName
        {
            get { return "audioConversionSetting"; }
        }

    	public string FFMpegPathSettingName
    	{
			get { return "FFMpegPath"; }
    	}

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.AudioConversionElement.ToBitmapSource(); }
        }

        public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.AudioConversionElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof(AudioConversionElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (AudioConversionElement); }
        }
    }
}
