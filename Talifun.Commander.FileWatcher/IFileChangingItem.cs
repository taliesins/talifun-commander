using System;
using System.IO;

namespace Talifun.Commander.FileWatcher
{
    internal interface IFileChangingItem
    {
        DateTime FireTime
        {
            get;
            set;
        }

        FileSystemEventArgs FileSystemEventArgs
        {
            get;
        }
    }
}
