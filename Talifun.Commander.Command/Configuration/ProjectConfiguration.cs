using System;
using System.Drawing;

namespace Talifun.Commander.Command.Configuration
{
    public class ProjectConfiguration : ISettingConfiguration
    {
        private ProjectConfiguration()
        {
        }

        public static readonly ProjectConfiguration Instance = new ProjectConfiguration();

        public string ConversionType
        {
            get { throw new NotImplementedException(); }
        }

        public string CollectionSettingName
        {
            get { return "projects"; }
        }

        public string ElementSettingName
        {
            get { return "project"; }
        }

        public Image ElementImage
        {
            get { return Properties.Resource.project; }
        }

        public Image ElementCollectionImage
        {
            get { return Properties.Resource.projects; }
        }
    }
}
