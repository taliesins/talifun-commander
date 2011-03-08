using System;

namespace Talifun.Commander.FileWatcher
{
    public class FileCreatedEventArgs : EventArgs
    {
        public FileCreatedEventArgs(string filePath, object userState)
        {
            FilePath = filePath;
            UserState = userState;
        }

        public string FilePath { get; private set; }
        public object UserState { get; private set; }
    }
}
