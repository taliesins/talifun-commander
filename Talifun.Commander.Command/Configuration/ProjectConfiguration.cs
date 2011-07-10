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

        public string ElementCollectionSettingName
        {
            get { return "projects"; }
        }

        public string ElementSettingName
        {
            get { return "project"; }
        }

        public Bitmap ElementImage
        {
            get { return Properties.Resource.ProjectElement; }
        }

        public Bitmap ElementCollectionImage
        {
            get { return Properties.Resource.ProjectElementCollection; }
        }

        public Type ElementCollectionType
        {
            get { return typeof(ProjectElementCollection); }
        }

        public Type ElementType
        {
            get { return typeof(ProjectElement); }
        }
    }
}
