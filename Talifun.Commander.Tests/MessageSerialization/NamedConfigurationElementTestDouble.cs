﻿using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Tests.MessageSerialization
{
	[JsonObject(MemberSerialization.OptIn)]
	public sealed class NamedConfigurationElementTestDouble : NamedConfigurationElement
	{
		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private static readonly ConfigurationProperty name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		/// <summary>
		/// Initializes the <see cref="NamedConfigurationElementTestDouble"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static NamedConfigurationElementTestDouble()
        {
            properties.Add(name);
        }

		/// <summary>
		/// Gets or sets the name of the configuration element represented by this instance.
		/// </summary>
		[ConfigurationProperty("name", DefaultValue = null, IsRequired = true, IsKey = true)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public override string Name
		{
			get { return ((string)base[name]); }
			set { SetPropertyValue(value, name, "Name"); }
		}
	}
}
