using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.Configuration
{
    public class FolderConfiguration : ISettingConfiguration
    {
        private FolderConfiguration()
        {
        }

        public static readonly FolderConfiguration Instance = new FolderConfiguration();

        public string ConversionType
        {
            get { throw new NotImplementedException(); }
        }

        public string ElementCollectionSettingName
        {
            get { return "folders"; }
        }

        public string ElementSettingName
        {
            get { return "folder"; }
        }

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.FolderElement.ToBitmapSource(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.FolderElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof(FolderElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof (FolderElement); }
        }
    }
}
