using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Configuration
{
    public class FileMatchConfiguration : ISettingConfiguration
    {
        private FileMatchConfiguration()
        {
        }

        public static readonly FileMatchConfiguration Instance = new FileMatchConfiguration();

        public string ConversionType
        {
            get { throw new NotImplementedException(); }
        }

        public string ElementCollectionSettingName
        {
            get { return "fileMatches"; }
        }

        public string ElementSettingName
        {
            get { return "fileMatch"; }
        }

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.FileMatchElement.ToBitmapSource(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.FileMatchElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof (FileMatchElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (FileMatchElement); }
        }
    }
}
