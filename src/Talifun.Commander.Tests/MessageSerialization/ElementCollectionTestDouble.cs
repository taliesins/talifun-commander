using System.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Tests.MessageSerialization
{
	/// <summary>
	/// Represents a configuration element containing a collection of <see cref="FolderElement" /> configuration elements.
	/// </summary>
	[ConfigurationCollection(typeof(FolderElement))]
	public class ElementCollectionTestDouble : CurrentConfigurationElementCollection<ElementTestDouble>
	{
		public ElementCollectionTestDouble()
		{
			Setting = ElementConfiguration.Instance;
			AddElementName = Setting.ElementSettingName;
		}

		public override ConfigurationProperty CreateNewConfigurationProperty()
		{
			return new ConfigurationProperty(Setting.ElementCollectionSettingName, typeof(FolderElementCollection), new FolderElementCollection(), ConfigurationPropertyOptions.None);
		}
	}
}
