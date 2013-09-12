using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.PicasaUploader.Configuration
{
	/// <summary>
	/// Represents a configuration element containing a collection of <see cref="PicasaUploaderElement" /> configuration elements.
	/// </summary>
	[ConfigurationCollection(typeof(PicasaUploaderElement))]
	public class PicasaUploaderElementCollection : CurrentConfigurationElementCollection<PicasaUploaderElement>
	{
		public PicasaUploaderElementCollection()
        {
			Setting = PicasaUploaderConfiguration.Instance;
            AddElementName = Setting.ElementSettingName;
        }

        public override ConfigurationProperty CreateNewConfigurationProperty()
        {
			return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(PicasaUploaderElementCollection), new PicasaUploaderElementCollection(), ConfigurationPropertyOptions.None);
        }
	}
}
