using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.Command;

namespace Talifun.Commander.Tests.MessageSerialization
{
	public class ElementConfiguration : ISettingConfiguration
	{
		private ElementConfiguration()
        {
        }

		public static readonly ElementConfiguration Instance = new ElementConfiguration();

        public string ConversionType
        {
            get { throw new NotImplementedException(); }
        }

        public string ElementCollectionSettingName
        {
            get { return "elements"; }
        }

        public string ElementSettingName
        {
            get { return "element"; }
        }

        public BitmapSource ElementImage
        {
			get { throw new NotImplementedException(); }
        }

		public BitmapSource ElementCollectionImage
        {
			get { throw new NotImplementedException(); }
        }

        public Type ElementCollectionType
        {
            get { return typeof (ElementCollectionTestDouble); }
        }

        public Type ElementType
        {
            get { return typeof (ElementTestDouble); }
        }
	}
}
