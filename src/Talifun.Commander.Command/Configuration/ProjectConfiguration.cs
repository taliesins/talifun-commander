using System;
using System.Windows.Media.Imaging;
using Talifun.Commander.UI;

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

        public BitmapSource ElementImage
        {
			get { return Properties.Resource.ProjectElement.ToBitmapSource(); }
        }

        public BitmapSource ElementCollectionImage
        {
			get { return Properties.Resource.ProjectElementCollection.ToBitmapSource(); }
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
