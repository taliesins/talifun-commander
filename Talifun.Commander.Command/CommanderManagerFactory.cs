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
            return new CommanderManager();
        }

        #endregion
    }
}
