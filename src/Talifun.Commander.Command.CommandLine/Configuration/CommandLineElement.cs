using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
	[JsonObject(MemberSerialization.OptIn)]
    public sealed partial class CommandLineElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty commandPath = new ConfigurationProperty("commandPath", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty checkCommandPathExists = new ConfigurationProperty("checkCommandPathExists", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty args = new ConfigurationProperty("args", typeof(string), "", ConfigurationPropertyOptions.None);

        /// <summary>
		/// Initializes the <see cref="CommandLineElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static CommandLineElement()
        {
            properties.Add(name);
            properties.Add(outPutPath);
            properties.Add(workingPath);
            properties.Add(errorProcessingPath);
            properties.Add(fileNameFormat);

            properties.Add(commandPath);
            properties.Add(checkCommandPathExists);
            properties.Add(args);
        }

		public CommandLineElement()
		{
			Setting = CommandLineConfiguration.Instance;	
		}

        /// <summary>
        /// Gets or sets the command path to use. e.g. c:\bin\test.exe
        /// </summary>
        [ConfigurationProperty("commandPath", DefaultValue = "", IsRequired = true)]
		[JsonProperty]
        public string CommandPath
        {
            get { return ((string)base[commandPath]); }
			set { SetPropertyValue(value, commandPath, "CommandPath"); }
        }

        /// <summary>
        /// Gets or sets if the command path should be checked for its existance
        /// </summary>
        [ConfigurationProperty("checkCommandPathExists", DefaultValue = true, IsRequired = false)]
		[JsonProperty]
        public bool CheckCommandPathExists
        {
            get { return ((bool)base[checkCommandPathExists]); }
			set { SetPropertyValue(value, checkCommandPathExists, "CheckCommandPathExists"); }
        }
        
        /// <summary>
        /// Gets or sets the command line arguments to use. Tokens will be replaced in argument string.
        /// </summary>
        /// <remarks>
        /// This tokens will be replaced with their settings value
        /// {%Name%}
        /// {%OutPutPath%}
        /// {%WorkingPath%}
        /// {%ErrorProcessingPath%}
        /// {%FileNameFormat%}
        /// {%CommandPath%}
        /// {%InputFilePath%}
        /// {%OutputFilePath%}
        /// </remarks>
        [ConfigurationProperty("args", DefaultValue = "", IsRequired = false)]
		[JsonProperty]
        public string Args
        {
            get { return ((string)base[args]); }
			set { SetPropertyValue(value, args, "Args"); }
        }
    }
}
