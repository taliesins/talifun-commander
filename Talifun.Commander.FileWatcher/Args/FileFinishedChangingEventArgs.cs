using System;
using System.IO;

namespace Talifun.Commander.FileWatcher
{
    public class FileFinishedChangingEventArgs : EventArgs
    {
        public FileFinishedChangingEventArgs(string filePath, WatcherChangeTypes changeType, object userState)
        {
            FilePath = filePath;
            ChangeType = changeType;
            UserState = userState;
        }

        public string FilePath { get; private set; }
        public WatcherChangeTypes ChangeType { get; private set; }
        public object UserState { get; private set; }
    }
}
