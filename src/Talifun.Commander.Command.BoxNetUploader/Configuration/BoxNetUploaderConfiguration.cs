using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.Command.BoxNetUploader.Properties;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.BoxNetUploader.Configuration
{
	public class BoxNetUploaderConfiguration : ISettingConfiguration
	{
		private BoxNetUploaderConfiguration()
        {
        }

		public static readonly BoxNetUploaderConfiguration Instance = new BoxNetUploaderConfiguration();

        public string ConversionType
        {
			get { return "BoxNetUploader"; }
        }

        public string ElementCollectionSettingName
        {
			get { return "boxNetUploaderSettings"; }
        }

        public string ElementSettingName
        {
			get { return "boxNetUploaderSetting"; }
        }

        public BitmapSource ElementImage
        {
			get { return Resource.BoxNetUploaderElement.ToBitmapSource(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { return Resource.BoxNetUploaderElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
			get { return typeof(BoxNetUploaderElementCollection); }
        }

        public Type ElementType
        {
			get { return typeof(BoxNetUploaderElement); }
        }
	}
}
