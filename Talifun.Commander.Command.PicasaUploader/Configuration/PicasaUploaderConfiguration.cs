using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.Command.PicasaUploader.Properties;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.PicasaUploader.Configuration
{
	public class PicasaUploaderConfiguration : ISettingConfiguration
	{
		private PicasaUploaderConfiguration()
        {
        }

		public static readonly PicasaUploaderConfiguration Instance = new PicasaUploaderConfiguration();

        public string ConversionType
        {
			get { return "PicasaUploader"; }
        }

        public string ElementCollectionSettingName
        {
			get { return "picasaUploaderSettings"; }
        }

        public string ElementSettingName
        {
			get { return "picasaUploaderSetting"; }
        }

        public BitmapSource ElementImage
        {
			get { return Resource.PicasaUploaderElement.ToBitmapSource(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { return Resource.PicasaUploaderElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
			get { return typeof(PicasaUploaderElementCollection); }
        }

        public Type ElementType
        {
			get { return typeof(PicasaUploaderElement); }
        }
	}
}
