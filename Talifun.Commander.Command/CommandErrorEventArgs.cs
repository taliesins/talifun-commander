using System;

namespace Talifun.Commander.Command
{
    public class CommandErrorEventArgs : EventArgs
    {
        public CommandErrorEventArgs(string exceptionMessage)
        {
            ExceptionMessage = exceptionMessage;
        }

        public string ExceptionMessage { get; private set; }
    }
}