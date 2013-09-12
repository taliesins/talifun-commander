using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	public class YouTubeUploaderConfiguration : ISettingConfiguration
	{
		private YouTubeUploaderConfiguration()
        {
        }

        public static readonly YouTubeUploaderConfiguration Instance = new YouTubeUploaderConfiguration();

        public string ConversionType
        {
            get { return "YouTubeUploader"; }
        }

        public string ElementCollectionSettingName
        {
            get { return "youTubeUploaderSettings"; }
        }

        public string ElementSettingName
        {
            get { return "youTubeUploaderSetting"; }
        }

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.YouTubeUploaderElement.ToBitmapSource(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.YouTubeUploaderElementCollection.ToBitmapSource(); }
        }

        public Type ElementCollectionType
        {
			get { return typeof(YouTubeUploaderElementCollection); }
        }

        public Type ElementType
        {
			get { return typeof(YouTubeUploaderElement); }
        }
	}
}
