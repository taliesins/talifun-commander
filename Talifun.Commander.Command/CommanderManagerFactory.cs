using Talifun.Commander.Command.Configuration;
using Talifun.Commander.FileWatcher;

namespace Talifun.Commander.Command
{
    public sealed class CommanderManagerFactory : ICommanderManagerFactory
    {
        private CommanderManagerFactory()
        {
        }

        public static readonly ICommanderManagerFactory Instance = new CommanderManagerFactory();

        #region ICommanderManagerFactory Members
        public ICommanderManager CreateCommandManager()
        {
			return new CommanderManager(CommandContainer.Instance.Container, CurrentConfiguration.CommanderConfiguration, CurrentConfiguration.AppSettings, EnhancedFileSystemWatcherFactory.Instance);
        }

		public ICommanderManager CreateCommandManager(System.Configuration.Configuration configuration)
		{
			CurrentConfiguration.Configuration = configuration;
			return new CommanderManager(CommandContainer.Instance.Container, CurrentConfiguration.CommanderConfiguration, CurrentConfiguration.AppSettings, EnhancedFileSystemWatcherFactory.Instance);
		}

        #endregion
    }
}
