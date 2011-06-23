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

        public string CollectionSettingName
        {
            get { return "folders"; }
        }

        public string ElementSettingName
        {
            get { return "folder"; }
        }

        public Image ElementImage
        {
            get { return Properties.Resource.folder; }
        }

        public Image ElementCollectionImage
        {
            get { return Properties.Resource.folders; }
        }
    }
}
