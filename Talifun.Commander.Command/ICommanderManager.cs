using System;
using System.IO;

namespace Talifun.Commander.Command
{
    public interface ICommanderManager
    {
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
