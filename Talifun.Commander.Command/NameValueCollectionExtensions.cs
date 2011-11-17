using System.Collections.Generic;
using System.Configuration;

namespace Talifun.Commander.Command
{
	public static class NameValueCollectionExtensions
	{
		public static Dictionary<string, string> ToDictionary(this KeyValueConfigurationCollection collection)
		{
			var dictionary = new Dictionary<string, string>();
			foreach (var key in collection.AllKeys)
			{
				dictionary[key] = collection[key].Value;
			}

			return dictionary;
		}
	}
}
