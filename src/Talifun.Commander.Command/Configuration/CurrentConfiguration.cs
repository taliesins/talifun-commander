using System.ComponentModel.Composition.Hosting;
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

		internal static System.Configuration.Configuration Configuration { private get; set; }

		/// <summary>
		/// Get the container for plugins.
		/// </summary>
		public static ExportProvider Container { get; internal set; }

		/// <summary>
		/// Gets the current application configuration.
		/// </summary>
		public static AppSettingsSection AppSettings
		{
			get { return Configuration.AppSettings; }
		}

    	public static IDefaultPaths DefaultPaths
    	{
			get { return new DefaultPaths(AppSettings, CommanderSettings); }
    	}

    	/// <summary>
        /// Gets the current commander configuration.
        /// </summary>
        public static CommanderSection CommanderSettings
        {
			get
			{
				return CurrentConfigurationManager.GetSection<CommanderSection>(Configuration);
			}
        }
    }
}
