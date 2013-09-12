using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.Command.DropBoxUploader.Properties;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
	public class DropBoxUploaderConfiguration : ISettingConfiguration
	{
		private DropBoxUploaderConfiguration()
        {
        }

		public static readonly DropBoxUploaderConfiguration Instance = new DropBoxUploaderConfiguration();

        public string ConversionType
        {
			get { return "DropBoxUploader"; }
        }

        public string ElementCollectionSettingName
        {
			get { return "dropBoxUploaderSettings"; }
        }

        public string ElementSettingName
        {
			get { return "dropBoxUploaderSetting"; }
        }

        public BitmapSource ElementImage
        {
			get { return Resource.DropBoxUploaderElement.ToBitmapSource(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { return Resource.DropBoxUploaderElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
			get { return typeof(DropBoxUploaderElementCollection); }
        }

        public Type ElementType
        {
			get { return typeof(DropBoxUploaderElement); }
        }
	}
}
