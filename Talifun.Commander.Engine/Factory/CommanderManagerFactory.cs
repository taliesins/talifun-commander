namespace Talifun.Commander.MediaConversion
{
    public sealed class CommanderManagerFactory : ICommanderManagerFactory
    {
        private CommanderManagerFactory()
        {
        }

        public static readonly ICommanderManagerFactory Instance = new CommanderManagerFactory();

        #region ICommanderManagerFactory Members
        public ICommanderManager GetMediaConversionManager()
        {
            return new CommanderManager();
        }

        #endregion
    }
}
