using System;
using System.Drawing;

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

        public Bitmap ElementImage
        {
            get { return Properties.Resource.FileMatchElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.FileMatchElementCollection; }
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
