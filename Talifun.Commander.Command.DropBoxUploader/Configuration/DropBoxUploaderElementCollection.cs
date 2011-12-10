using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element containing a collection of <see cref="DropBoxUploaderElement" /> configuration elements.
	/// </summary>
	[ConfigurationCollection(typeof(DropBoxUploaderElement))]
	public class DropBoxUploaderElementCollection : CurrentConfigurationElementCollection<DropBoxUploaderElement>
	{
		public DropBoxUploaderElementCollection()
        {
			Setting = DropBoxUploaderConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
			return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(DropBoxUploaderElementCollection), new DropBoxUploaderElementCollection(), ConfigurationPropertyOptions.None);
        }
	}
}
