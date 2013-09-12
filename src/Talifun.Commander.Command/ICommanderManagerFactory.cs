namespace Talifun.Commander.Command
{
    public interface ICommanderManagerFactory
    {
        ICommanderManager CreateCommandManager();
    	ICommanderManager CreateCommandManager(System.Configuration.Configuration configuration);
    }
}
