using System;
using System.Drawing;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
    public class CommandLineConfiguration : ISettingConfiguration
    {
        private CommandLineConfiguration()
        {
        }

        public static readonly CommandLineConfiguration Instance = new CommandLineConfiguration();

        public string ConversionType
        {
            get { return "CommandLine"; }
        }

        public string ElementCollectionSettingName
        {
            get { return "commandLineSettings"; }
        }

        public string ElementSettingName
        {
            get { return "commandLineSetting"; }
        }

        public Bitmap ElementImage
        {
            get { return Properties.Resource.CommandLineElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.CommandLineElementCollection; }
        }

        public Type ElementCollectionType
        {
            get { return typeof (CommandLineSettingElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (CommandLineSettingElement); }
        }
    }
}
