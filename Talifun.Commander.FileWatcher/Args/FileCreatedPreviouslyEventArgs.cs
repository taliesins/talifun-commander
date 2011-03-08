using System;

namespace Talifun.Commander.FileWatcher
{
    public class FileCreatedPreviouslyEventArgs : EventArgs
    {
        public FileCreatedPreviouslyEventArgs(string filePath, object userState)
        {
            FilePath = filePath;
            UserState = userState;
        }

        public string FilePath{get; private set;}
        public object UserState{get; private set;}
    }
}
