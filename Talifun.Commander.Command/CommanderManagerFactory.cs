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
            return new CommanderManager(CommandContainer.Instance.Container, CurrentConfiguration.Current, EnhancedFileSystemWatcherFactory.Instance);
        }

        #endregion
    }
}
