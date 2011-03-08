namespace Talifun.Commander.Configuration
{
    /// <summary>
    /// Provides easy access to the current application configuration.
    /// </summary>
    public static class CurrentConfiguration
    {
        /// <summary>
        /// Gets the static instance of <see cref="CommanderSection" /> representing the current application configuration.
        /// </summary>
        public static CommanderSection Current
        {
            get { return CurrentConfigurationManager.GetSection<CommanderSection>(); }
        }
    }
}
