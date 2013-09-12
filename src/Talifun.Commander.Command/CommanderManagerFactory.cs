using Talifun.Commander.Command.Configuration;
using Talifun.FileWatcher;

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
        	CurrentConfiguration.Container = CommandContainer.Instance.Container;
			return new CommanderManager(CurrentConfiguration.Container, CurrentConfiguration.AppSettings, CurrentConfiguration.CommanderSettings, EnhancedFileSystemWatcherFactory.Instance);
        }

		public ICommanderManager CreateCommandManager(System.Configuration.Configuration configuration)
		{
			CurrentConfiguration.Container = CommandContainer.Instance.Container;
			CurrentConfiguration.Configuration = configuration;
			return new CommanderManager(CurrentConfiguration.Container, CurrentConfiguration.AppSettings, CurrentConfiguration.CommanderSettings, EnhancedFileSystemWatcherFactory.Instance);
		}

        #endregion
    }
}
