using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    public interface ICommanderManager
    {
        ExportProvider Container { get; }
        CommanderSection Configuration { get;  }
        
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
