using System;
using System.IO;
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

        void LogException(FileInfo errorFileInfo, Exception exception);

        /// <summary>
        /// This event occurs when their is an error executing a command.
        /// </summary>
        event CommandErrorEventHandler CommandErrorEvent;
    }
}
