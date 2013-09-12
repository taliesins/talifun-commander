using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FlickrUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element containing a collection of <see cref="FlickrUploaderElement" /> configuration elements.
	/// </summary>
	[ConfigurationCollection(typeof(FlickrUploaderElement))]
	public class FlickrUploaderElementCollection : CurrentConfigurationElementCollection<FlickrUploaderElement>
	{
		public FlickrUploaderElementCollection()
        {
			Setting = FlickrUploaderConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
			return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(FlickrUploaderElementCollection), new FlickrUploaderElementCollection(), ConfigurationPropertyOptions.None);
        }
	}
}
