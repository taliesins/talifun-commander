using System;

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

        public string CollectionSettingName
        {
            get { return "fileMatches"; }
        }

        public string ElementSettingName
        {
            get { return "fileMatch"; }
        }

        public System.Drawing.Image ElementImage
        {
            get { return Properties.Resource.fileMatch; }
        }


        public System.Drawing.Image ElementCollectionImage
        {
            get { return Properties.Resource.fileMatches; }
        }
    }
}
