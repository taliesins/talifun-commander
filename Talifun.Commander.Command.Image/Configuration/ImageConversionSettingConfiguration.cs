﻿using System.Configuration;

namespace Talifun.Commander.Command.Image.Configuration
{
    public static class ImageConversionSettingConfiguration
    {
        public const string ConversionType = "ImageConversion";
        public const string CollectionSettingName = "imageConversionSettings";
        public const string ElementSettingName = "imageConversionSetting";

        public static string ConvertPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ConvertPath"];
            }
        }
    }
}
