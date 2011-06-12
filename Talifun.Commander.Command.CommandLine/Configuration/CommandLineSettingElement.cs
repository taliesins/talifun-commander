﻿using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Talifun.Commander.Command.CommandLine
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public sealed class CommandLineSettingElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty commandPath = new ConfigurationProperty("commandPath", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty checkCommandPathExists = new ConfigurationProperty("checkCommandPathExists", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty args = new ConfigurationProperty("args", typeof(string), "", ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="AudioConversionSettingElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static CommandLineSettingElement()
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

        /// <summary>
        /// Gets or sets the command path to use. e.g. c:\bin\test.exe
        /// </summary>
        [ConfigurationProperty("commandPath", DefaultValue = "", IsRequired = true)]
        public string CommandPath
        {
            get { return ((string)base[commandPath]); }
            set { base[commandPath] = value; }
        }

        /// <summary>
        /// Gets or sets if the command path should be checked for its existance
        /// </summary>
        [ConfigurationProperty("checkCommandPathExists", DefaultValue = true, IsRequired = false)]
        public bool CheckCommandPathExists
        {
            get { return ((bool)base[checkCommandPathExists]); }
            set { base[checkCommandPathExists] = value; }
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
        public string Args
        {
            get { return ((string)base[args]); }
            set { base[args] = value; }
        }
    }
}
