﻿using System.Configuration;

namespace Talifun.Commander.Command.Audio.Configuration
{
    public static class AudioConversionSettingConfiguration
    {
        public const string ConversionType = "AudioConversion";
        public const string CollectionSettingName = "audioConversionSettings";
        public const string ElementSettingName = "audioConversionSetting";

        public static string FFMpegPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FFMpegPath"];
            }
        }
    }
}
