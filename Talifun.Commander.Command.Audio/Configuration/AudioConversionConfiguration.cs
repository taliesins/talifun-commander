﻿using System;
using System.Configuration;
using System.Drawing;

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

        public string FFMpegPath
        {
            get
            {
				return ConfigurationManager.AppSettings[FFMpegPathSettingName];
            }
        }

        public Bitmap ElementImage
        {
            get { return Properties.Resource.AudioConversionElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.AudioConversionElementCollection; }
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
