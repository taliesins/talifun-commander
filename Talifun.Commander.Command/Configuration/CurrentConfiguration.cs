using System.Configuration;

namespace Talifun.Commander.Command.Configuration
{
    /// <summary>
    /// Provides easy access to the current application configuration. This will allow static configuration elements to access configuration.
    /// </summary>
    public static class CurrentConfiguration
    {
		static CurrentConfiguration()
		{
			Configuration = CurrentConfigurationManager.GetCurrentConfiguration();
		}

    	/// <summary>
        /// Gets the static instance of <see cref="CommanderSection" /> representing the current application configuration.
        /// </summary>
        public static CommanderSection CommanderConfiguration
        {
			get
			{
				return CurrentConfigurationManager.GetSection<CommanderSection>(Configuration);
			}
        }

		/// <summary>
		/// Gets the static instance of <see cref="AppSettingsSection" /> representing the current application configuration.
		/// </summary>
    	public static AppSettingsSection AppSettings
    	{
    		get { return Configuration.AppSettings; }
    	}

    	public static System.Configuration.Configuration Configuration { get; internal set; }
    }
}
