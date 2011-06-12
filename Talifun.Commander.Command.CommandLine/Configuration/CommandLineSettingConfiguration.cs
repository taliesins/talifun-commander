﻿namespace Talifun.Commander.Command.CommandLine.Configuration
{
    public class CommandLineSettingConfiguration : ISettingConfiguration
    {
        private CommandLineSettingConfiguration()
        {
        }

        public static readonly CommandLineSettingConfiguration Instance = new CommandLineSettingConfiguration();

        public string ConversionType
        {
            get { return "CommandLine"; }
        }

        public string CollectionSettingName
        {
            get { return "commandLineSettings"; }
        }

        public string ElementSettingName
        {
            get { return "commandLineSetting"; }
        }
    }
}
