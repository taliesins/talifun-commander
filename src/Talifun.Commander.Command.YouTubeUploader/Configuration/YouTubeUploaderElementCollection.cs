using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element containing a collection of <see cref="YouTubeUploaderElement" /> configuration elements.
	/// </summary>
	[ConfigurationCollection(typeof(YouTubeUploaderElement))]
	public class YouTubeUploaderElementCollection : CurrentConfigurationElementCollection<YouTubeUploaderElement>
	{
		public YouTubeUploaderElementCollection()
        {
			Setting = YouTubeUploaderConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
			return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(YouTubeUploaderElementCollection), new YouTubeUploaderElementCollection(), ConfigurationPropertyOptions.None);
        }
	}
}
