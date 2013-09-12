using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.BoxNetUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element containing a collection of <see cref="BoxNetUploaderElement" /> configuration elements.
	/// </summary>
	[ConfigurationCollection(typeof(BoxNetUploaderElement))]
	public class BoxNetUploaderElementCollection : CurrentConfigurationElementCollection<BoxNetUploaderElement>
	{
		public BoxNetUploaderElementCollection()
        {
			Setting = BoxNetUploaderConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
			return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(BoxNetUploaderElementCollection), new BoxNetUploaderElementCollection(), ConfigurationPropertyOptions.None);
        }
	}
}
