using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.Command.FlickrUploader.Properties;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.FlickrUploader.Configuration
{
	public class FlickrUploaderConfiguration : ISettingConfiguration
	{
		private FlickrUploaderConfiguration()
        {
        }

		public static readonly FlickrUploaderConfiguration Instance = new FlickrUploaderConfiguration();

        public string ConversionType
        {
			get { return "FlickrUploader"; }
        }

        public string ElementCollectionSettingName
        {
			get { return "flickrUploaderSettings"; }
        }

        public string ElementSettingName
        {
			get { return "flickrUploaderSetting"; }
        }

        public BitmapSource ElementImage
        {
			get { return Resource.FlickrUploaderElement.ToBitmapSource(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { return Resource.FlickrUploaderElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
			get { return typeof(FlickrUploaderElementCollection); }
        }

        public Type ElementType
        {
			get { return typeof(FlickrUploaderElement); }
        }
	}
}
