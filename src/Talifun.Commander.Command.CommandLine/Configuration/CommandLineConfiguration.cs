using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

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

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.CommandLineElement.ToBitmapSource(); }
        }

        public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.CommandLineElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof (CommandLineElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (CommandLineElement); }
        }
    }
}
