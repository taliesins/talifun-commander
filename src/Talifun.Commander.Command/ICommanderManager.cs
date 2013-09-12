using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    public interface ICommanderManager
    {
    	CommanderSectionWindow GetCommanderSectionWindow();
        
        void Start();
        void Stop();
        bool IsRunning
        {
            get;
        }

    	event CommanderStartedEventHandler CommanderStartedEvent;
    	event CommanderStoppedEventHandler CommanderStoppedEvent;
    }
}
