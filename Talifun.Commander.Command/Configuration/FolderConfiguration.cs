using System;
using System.Drawing;

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

        public Bitmap ElementImage
        {
            get { return Properties.Resource.FolderElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.FolderElementCollection; }
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
