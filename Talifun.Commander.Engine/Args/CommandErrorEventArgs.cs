using System;

namespace Talifun.Commander.MediaConversion
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