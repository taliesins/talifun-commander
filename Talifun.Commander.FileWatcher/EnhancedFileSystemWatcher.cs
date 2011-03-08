using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Timers;

namespace Talifun.Commander.FileWatcher
{
    internal class EnhancedFileSystemWatcher : IEnhancedFileSystemWatcher
    {
        private TimeSpan m_LockTimeout = TimeSpan.FromSeconds(10);
        private AsyncOperation m_AsyncOperation = AsyncOperationManager.CreateOperation(null);

        private List<string> m_FileToBeChecked = null;

        private readonly object m_FilesRaisingEventsLock = new object();
        private Dictionary<string, IFileChangingItem> m_FilesChanging;

        private System.Timers.Timer m_Timer = new System.Timers.Timer();
        private string m_NextFileToCheck = string.Empty;

        private FileSystemEventHandler m_FileSystemWatcherChangedEvent;
        private FileSystemEventHandler m_FileSystemWatcherCreatedEvent;
        private FileSystemEventHandler m_FileSystemWatcherDeletedEvent;
        private RenamedEventHandler m_FileSystemWatcherRenamedEvent;
        private FileFinishedChangingCallback m_FileFinishedChangingCallback;
        private FileSystemWatcher m_FileSystemWatcher;

        private bool m_IncludeSubdirectories = false;
        private string m_FolderToWatch;
        private string m_Filter;
        private int m_PollTime;
        private object m_UserState;

        public EnhancedFileSystemWatcher(string folderToWatch, string filter, int pollTime, bool includeSubdirectories)
        {
            m_FolderToWatch = folderToWatch;
            m_Filter = filter;
            m_PollTime = pollTime;
            m_IncludeSubdirectories = includeSubdirectories;

            m_Timer = new System.Timers.Timer();
            m_Timer.Elapsed += new ElapsedEventHandler(OnTimeUp);
            m_Timer.Interval = m_PollTime;
            m_Timer.Enabled = true;
            m_Timer.AutoReset = false;

            m_FileSystemWatcherChangedEvent = new FileSystemEventHandler(OnFileChanged);
            m_FileSystemWatcherCreatedEvent = new FileSystemEventHandler(OnFileCreated);
            m_FileSystemWatcherDeletedEvent = new FileSystemEventHandler(OnFileDeleted);
            m_FileSystemWatcherRenamedEvent = new RenamedEventHandler(OnFileRenamed);
            m_FileFinishedChangingCallback = new FileFinishedChangingCallback(OnFileFinishedChanging);

            if (!string.IsNullOrEmpty(m_Filter))
            {
                m_FileSystemWatcher = new FileSystemWatcher(m_FolderToWatch, m_Filter);
            }
            else
            {
                m_FileSystemWatcher = new FileSystemWatcher(m_FolderToWatch);
            }
            m_FileSystemWatcher.IncludeSubdirectories = m_IncludeSubdirectories;
            m_FileSystemWatcher.EnableRaisingEvents = false;
            m_FileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            m_FileSystemWatcher.Changed += m_FileSystemWatcherChangedEvent;
            m_FileSystemWatcher.Created += m_FileSystemWatcherCreatedEvent;
            m_FileSystemWatcher.Deleted += m_FileSystemWatcherDeletedEvent;
            m_FileSystemWatcher.Renamed += m_FileSystemWatcherRenamedEvent;
        }

        public void Start()
        {
            if (!m_FileSystemWatcher.EnableRaisingEvents)
            {
                m_FilesChanging = new Dictionary<string, IFileChangingItem>();
                m_FileSystemWatcher.EnableRaisingEvents = true;

                RaiseEventsForExistingFiles();
            }
        }

        public void Stop()
        {
            if (m_FileSystemWatcher.EnableRaisingEvents)
            {
                m_FileSystemWatcher.EnableRaisingEvents = false;

                Thread.Sleep(0);

                lock (m_FilesRaisingEventsLock)
                {
                    m_Timer.Stop();
                    m_NextFileToCheck = string.Empty;
                    m_FilesChanging.Clear();
                }
            }
        }

        public object UserState
        {
            get
            {
                return m_UserState;
            }
            set
            {
                m_UserState = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return m_FileSystemWatcher.EnableRaisingEvents;
            }
        }

        public string FolderToWatch
        {
            get
            {
                return m_FolderToWatch;
            }
        }

        public string Filter
        {
            get
            {
                return m_Filter;
            }
        }

        public int PollTime
        {
            get
            {
                return m_PollTime;
            }
        }

        public bool IncludeSubdirectories
        {
            get
            {
                return m_IncludeSubdirectories;
            }
        }

        private void RaiseEventsForExistingFiles()
        {
            m_FileToBeChecked = new List<string>();
            GetAllFilesToCheck(m_FolderToWatch);
            CheckFiles();
            m_FileToBeChecked = null;
        }

        private void GetAllFilesToCheck(string folderPath)
        {
            if (folderPath != null && folderPath.Length > 0)
            {
                // search in subdirectories
                if (m_IncludeSubdirectories)
                {
                    var folders = Directory.GetDirectories(folderPath);
                    foreach (string folder in folders)
                    {
                        GetAllFilesToCheck(folder);
                    }
                }

                string[] files = null;
                if (!string.IsNullOrEmpty(m_Filter))
                {
                    files = Directory.GetFiles(folderPath, m_Filter);
                }
                else
                {
                    files = Directory.GetFiles(folderPath);
                }

                foreach (var file in files)
                {
                    if (!IsFileLocked(file))
                    {
                        //File is not being used and is in the directory
                        m_FileToBeChecked.Add(file);
                    } else
                    {
                        //File is currently being used, but it might be free at a future date so put it on the queue
                        Push(file, new FileSystemEventArgs(WatcherChangeTypes.All, folderPath, file));
                    }
                }
            }
        }

        private void CheckFiles()
        {
            //Collection may be modified at any time, we have to enumerate throught the collection this way
            while (m_FileToBeChecked.Count > 0)
            {
                lock (m_FilesRaisingEventsLock)
                {
                    var filePath = m_FileToBeChecked[0];

                    var fileCreatedPreviouslyEventArgs = new FileCreatedPreviouslyEventArgs(filePath, m_UserState);
                    RaiseAsynchronousOnFileCreatedPreviouslyEvent(fileCreatedPreviouslyEventArgs);

                    m_FileToBeChecked.Remove(filePath);
                }
            }
        }

        private static bool IsFileLocked(string filePath)
        {
            var result = false;
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                result = true;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            return result;
        }

        #region FilesChanging Queue

        /// <summary>
        /// A file event has occured on a file already waiting to be raised, so update the time 
        /// the file event should next be fired.
        /// </summary>
        /// <param name="filePath"></param>
        private void Touch(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var item = m_FilesChanging[filePath];
            item.FireTime = DateTime.Now.AddMilliseconds(m_PollTime);

            if (m_NextFileToCheck == filePath)
            {
                GetNextFileToCheck();
            }
        }

        /// <summary>
        /// Add a file event that has occurred for a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="e"></param>
        private void Push(string filePath, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            IFileChangingItem item = new FileChangingItem(e);
            item.FireTime = DateTime.Now.AddMilliseconds(m_PollTime);
            m_FilesChanging.Add(filePath, item);

            if (string.IsNullOrEmpty(m_NextFileToCheck))
            {
                GetNextFileToCheck();
            }
        }

        /// <summary>
        /// Remove a file event.
        /// </summary>
        /// <param name="filePath"></param>
        private void Pop(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            m_FilesChanging.Remove(filePath);

            if (m_NextFileToCheck == filePath)
            {
                GetNextFileToCheck();
            }
        }

        private void GetNextFileToCheck()
        {
            m_Timer.Stop();

            var currentDateTime = DateTime.Now;

            m_NextFileToCheck = string.Empty;

            if (m_FilesChanging == null || m_FilesChanging.Count < 1) return;

            var lowestDateTime = DateTime.MaxValue;
            var nextFileToCheck = string.Empty;
            foreach (var item in m_FilesChanging)
            {
                var dateTime = item.Value.FireTime;

                if (currentDateTime < lowestDateTime)
                {
                    lowestDateTime = dateTime;
                    nextFileToCheck = item.Key;
                }
            }

            //There are no more files to raise events for
            if (string.IsNullOrEmpty(nextFileToCheck)) return;

            double interval = 1;

            if (lowestDateTime > currentDateTime)
            {
                interval = lowestDateTime.Subtract(currentDateTime).TotalMilliseconds;
            }

            if (interval < 1)
            {
                interval = 1;
            }

            m_NextFileToCheck = nextFileToCheck;
            m_Timer.Interval = interval;
            m_Timer.Start();
        }

        #endregion

        #region Events raised by file watcher

        private void OnTimeUp(object sender, ElapsedEventArgs e)
        {
            if (m_Timer != null)
            {
                lock (m_FilesRaisingEventsLock)
                {
                    m_Timer.Stop();

                    if (!string.IsNullOrEmpty(m_NextFileToCheck) && m_FilesChanging.ContainsKey(m_NextFileToCheck))
                    {
                        var item = m_FilesChanging[m_NextFileToCheck];
                        if (item.FireTime <= DateTime.Now)
                        {
                            m_FileFinishedChangingCallback(item.FileSystemEventArgs);
                        }
                    }

                    GetNextFileToCheck();
                }
            }
        }

        private void OnFileFinishedChanging(FileSystemEventArgs e)
        {
            var filePath = e.FullPath;

            lock (m_FilesRaisingEventsLock)
            {
                if (m_FileToBeChecked != null && m_FileToBeChecked.Contains(filePath))
                {
                    m_FileToBeChecked.Remove(filePath);
                }

                if (m_FilesChanging.ContainsKey(filePath))
                {
                    if (IsFileLocked(filePath))
                    {
                        //The file is still currently in use, lets try raise the event later
                        Touch(filePath);
                    }
                    else
                    {
                        Pop(filePath);
                    }
                }
            }

            var fileFinishedChangingEventArgs = new FileFinishedChangingEventArgs(filePath, WatcherChangeTypes.Changed, m_UserState);
            RaiseAsynchronousOnFileFinishedChangingEvent(fileFinishedChangingEventArgs);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;

            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                lock (m_FilesRaisingEventsLock)
                {
                    if (m_FileToBeChecked != null && m_FileToBeChecked.Contains(filePath))
                    {
                        m_FileToBeChecked.Remove(filePath);
                    }

                    if (m_FilesChanging.ContainsKey(filePath))
                    {
                        Touch(filePath);
                    }
                    else
                    {
                        Push(filePath, e);
                    }
                }

                var fileCreatedEvent = new FileCreatedEventArgs(e.FullPath, m_UserState);
                RaiseAsynchronousOnFileCreatedEvent(fileCreatedEvent);
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;

            lock (m_FilesRaisingEventsLock)
            {
                if (m_FileToBeChecked != null && m_FileToBeChecked.Contains(filePath))
                {
                    m_FileToBeChecked.Remove(filePath);
                }

                if (m_FilesChanging.ContainsKey(filePath))
                {
                    Touch(filePath);
                }
                else
                {
                    Push(filePath, e);
                }
            }

            var fileChangedEventArgs = new FileChangedEventArgs(filePath, m_UserState);
            RaiseAsynchronousOnFileChangedEvent(fileChangedEventArgs);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            string filePath = e.FullPath;

            lock (m_FilesRaisingEventsLock)
            {
                if (m_FileToBeChecked != null && m_FileToBeChecked.Contains(filePath))
                {
                    m_FileToBeChecked.Remove(filePath);
                }

                if (m_FilesChanging.ContainsKey(filePath))
                {
                    Pop(filePath);

                    var fileFinishedChangingEventArgs = new FileFinishedChangingEventArgs(filePath, WatcherChangeTypes.Deleted, m_UserState);
                    RaiseAsynchronousOnFileFinishedChangingEvent(fileFinishedChangingEventArgs);
                }
            }

            var fileDeletedEvent = new FileDeletedEventArgs(e.FullPath, m_UserState);
            RaiseAsynchronousOnFileDeletedEvent(fileDeletedEvent);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            var filePath = e.FullPath;

            lock (m_FilesRaisingEventsLock)
            {
                if (m_FileToBeChecked != null && m_FileToBeChecked.Contains(filePath))
                {
                    m_FileToBeChecked.Remove(filePath);
                }

                if (m_FilesChanging.ContainsKey(filePath))
                {
                    Pop(filePath);

                    var fileFinishedChangingEventArgs = new FileFinishedChangingEventArgs(filePath, WatcherChangeTypes.Renamed, m_UserState);
                    RaiseAsynchronousOnFileFinishedChangingEvent(fileFinishedChangingEventArgs);
                }
            }

            var fileRenamedEvent = new FileRenamedEventArgs(e.OldFullPath, e.FullPath, m_UserState);
            RaiseAsynchronousOnFileRenamedEvent(fileRenamedEvent);
        }

        #endregion

        #region FileCreatedPreviouslyEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private FileCreatedPreviouslyEventHandler m_FileCreatedPreviouslyEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object m_FileCreatedPreviouslyEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileCreatedPreviouslyEventHandler FileCreatedPreviouslyEvent
        {
            add
            {
                if (!Monitor.TryEnter(m_FileCreatedPreviouslyEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileCreatedPreviouslyEvent.add");
                }
                try
                {
                    m_FileCreatedPreviouslyEvent += value;
                }
                finally
                {
                    Monitor.Exit(m_FileCreatedPreviouslyEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(m_FileCreatedPreviouslyEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileCreatedPreviouslyEvent.remove");
                }
                try
                {
                    m_FileCreatedPreviouslyEvent -= value;
                }
                finally
                {
                    Monitor.Exit(m_FileCreatedPreviouslyEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        protected virtual void OnFileCreatedPreviouslyEvent(FileCreatedPreviouslyEventArgs e)
        {
            // TODO: Implement default behaviour of OnFileCreatedPreviouslyEvent
        }

        private void AsynchronousOnFileCreatedPreviouslyEventRaised(object state)
        {
            FileCreatedPreviouslyEventArgs e = state as FileCreatedPreviouslyEventArgs;
            RaiseOnFileCreatedPreviouslyEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileCreatedPreviouslyEvent(FileCreatedPreviouslyEventArgs e)
        {
            m_AsyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileCreatedPreviouslyEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileCreatedPreviouslyEvent(FileCreatedPreviouslyEventArgs e)
        {
            m_AsyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileCreatedPreviouslyEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnFileCreatedPreviouslyEvent(FileCreatedPreviouslyEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            FileCreatedPreviouslyEventHandler eventHandler;

            if (!Monitor.TryEnter(m_FileCreatedPreviouslyEventLock, m_LockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileCreatedPreviouslyEvent");
            }
            try
            {
                eventHandler = m_FileCreatedPreviouslyEvent;
            }
            finally
            {
                Monitor.Exit(m_FileCreatedPreviouslyEventLock);
            }

            OnFileCreatedPreviouslyEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region FileChangedEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private FileChangedEventHandler m_FileChangedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object m_FileChangedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileChangedEventHandler FileChangedEvent
        {
            add
            {
                if (!Monitor.TryEnter(m_FileChangedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileChangedEvent.add");
                }
                try
                {
                    m_FileChangedEvent += value;
                }
                finally
                {
                    Monitor.Exit(m_FileChangedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(m_FileChangedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileChangedEvent.remove");
                }
                try
                {
                    m_FileChangedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(m_FileChangedEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        private void OnFileChangedEvent(FileChangedEventArgs e)
        {
            // TODO: Implement default behaviour of OnFileChangedEvent
        }

        private void AsynchronousOnFileChangedEventRaised(object state)
        {
            FileChangedEventArgs e = state as FileChangedEventArgs;
            RaiseOnFileChangedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileChangedEvent(FileChangedEventArgs e)
        {
            m_AsyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileChangedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileChangedEvent(FileChangedEventArgs e)
        {
            m_AsyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileChangedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnFileChangedEvent(FileChangedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            FileChangedEventHandler eventHandler;

            if (!Monitor.TryEnter(m_FileChangedEventLock, m_LockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileChangedEvent");
            }
            try
            {
                eventHandler = m_FileChangedEvent;
            }
            finally
            {
                Monitor.Exit(m_FileChangedEventLock);
            }

            OnFileChangedEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region FileCreatedEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private FileCreatedEventHandler m_FileCreatedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object m_FileCreatedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileCreatedEventHandler FileCreatedEvent
        {
            add
            {
                if (!Monitor.TryEnter(m_FileCreatedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileCreatedEvent.add");
                }
                try
                {
                    m_FileCreatedEvent += value;
                }
                finally
                {
                    Monitor.Exit(m_FileCreatedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(m_FileCreatedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileCreatedEvent.remove");
                }
                try
                {
                    m_FileCreatedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(m_FileCreatedEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        private void OnFileCreatedEvent(FileCreatedEventArgs e)
        {
            // TODO: Implement default behaviour of OnFileCreatedEvent
        }

        private void AsynchronousOnFileCreatedEventRaised(object state)
        {
            FileCreatedEventArgs e = state as FileCreatedEventArgs;
            RaiseOnFileCreatedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileCreatedEvent(FileCreatedEventArgs e)
        {
            m_AsyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileCreatedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileCreatedEvent(FileCreatedEventArgs e)
        {
            m_AsyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileCreatedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnFileCreatedEvent(FileCreatedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            FileCreatedEventHandler eventHandler;

            if (!Monitor.TryEnter(m_FileCreatedEventLock, m_LockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileCreatedEvent");
            }
            try
            {
                eventHandler = m_FileCreatedEvent;
            }
            finally
            {
                Monitor.Exit(m_FileCreatedEventLock);
            }

            OnFileCreatedEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region FileDeletedEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private FileDeletedEventHandler m_FileDeletedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object m_FileDeletedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileDeletedEventHandler FileDeletedEvent
        {
            add
            {
                if (!Monitor.TryEnter(m_FileDeletedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileDeletedEvent.add");
                }
                try
                {
                    m_FileDeletedEvent += value;
                }
                finally
                {
                    Monitor.Exit(m_FileDeletedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(m_FileDeletedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileDeletedEvent.remove");
                }
                try
                {
                    m_FileDeletedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(m_FileDeletedEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        private void OnFileDeletedEvent(FileDeletedEventArgs e)
        {
            // TODO: Implement default behaviour of OnFileDeletedEvent
        }

        private void AsynchronousOnFileDeletedEventRaised(object state)
        {
            FileDeletedEventArgs e = state as FileDeletedEventArgs;
            RaiseOnFileDeletedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileDeletedEvent(FileDeletedEventArgs e)
        {
            m_AsyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileDeletedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileDeletedEvent(FileDeletedEventArgs e)
        {
            m_AsyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileDeletedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnFileDeletedEvent(FileDeletedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            FileDeletedEventHandler eventHandler;

            if (!Monitor.TryEnter(m_FileDeletedEventLock, m_LockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileDeletedEvent");
            }
            try
            {
                eventHandler = m_FileDeletedEvent;
            }
            finally
            {
                Monitor.Exit(m_FileDeletedEventLock);
            }

            OnFileDeletedEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region FileRenamedEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private FileRenamedEventHandler m_FileRenamedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object m_FileRenamedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileRenamedEventHandler FileRenamedEvent
        {
            add
            {
                if (!Monitor.TryEnter(m_FileRenamedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileRenamedEvent.add");
                }
                try
                {
                    m_FileRenamedEvent += value;
                }
                finally
                {
                    Monitor.Exit(m_FileRenamedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(m_FileRenamedEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileRenamedEvent.remove");
                }
                try
                {
                    m_FileRenamedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(m_FileRenamedEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        private void OnFileRenamedEvent(FileRenamedEventArgs e)
        {
            // TODO: Implement default behaviour of OnFileRenamedEvent
        }

        private void AsynchronousOnFileRenamedEventRaised(object state)
        {
            FileRenamedEventArgs e = state as FileRenamedEventArgs;
            RaiseOnFileRenamedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileRenamedEvent(FileRenamedEventArgs e)
        {
            m_AsyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileRenamedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileRenamedEvent(FileRenamedEventArgs e)
        {
            m_AsyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileRenamedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnFileRenamedEvent(FileRenamedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            FileRenamedEventHandler eventHandler;

            if (!Monitor.TryEnter(m_FileRenamedEventLock, m_LockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileRenamedEvent");
            }
            try
            {
                eventHandler = m_FileRenamedEvent;
            }
            finally
            {
                Monitor.Exit(m_FileRenamedEventLock);
            }

            OnFileRenamedEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region FileFinishedChangingEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private FileFinishedChangingEventHandler m_FileFinishedChangingEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object m_FileFinishedChangingEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileFinishedChangingEventHandler FileFinishedChangingEvent
        {
            add
            {
                if (!Monitor.TryEnter(m_FileFinishedChangingEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileFinishedChangingEvent.add");
                }
                try
                {
                    m_FileFinishedChangingEvent += value;
                }
                finally
                {
                    Monitor.Exit(m_FileFinishedChangingEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(m_FileFinishedChangingEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileFinishedChangingEvent.remove");
                }
                try
                {
                    m_FileFinishedChangingEvent -= value;
                }
                finally
                {
                    Monitor.Exit(m_FileFinishedChangingEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        private void OnFileFinishedChangingEvent(FileFinishedChangingEventArgs e)
        {
            // TODO: Implement default behaviour of OnFileFinishedChangingEvent
        }

        private void AsynchronousOnFileFinishedChangingEventRaised(object state)
        {
            FileFinishedChangingEventArgs e = state as FileFinishedChangingEventArgs;
            RaiseOnFileFinishedChangingEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileFinishedChangingEvent(FileFinishedChangingEventArgs e)
        {
            m_AsyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileFinishedChangingEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileFinishedChangingEvent(FileFinishedChangingEventArgs e)
        {
            m_AsyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileFinishedChangingEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnFileFinishedChangingEvent(FileFinishedChangingEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            FileFinishedChangingEventHandler eventHandler;

            if (!Monitor.TryEnter(m_FileFinishedChangingEventLock, m_LockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileFinishedChangingEvent");
            }
            try
            {
                eventHandler = m_FileFinishedChangingEvent;
            }
            finally
            {
                Monitor.Exit(m_FileFinishedChangingEventLock);
            }

            OnFileFinishedChangingEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region IDisposable Members
        private int m_AlreadyDisposed = 0;

        ~EnhancedFileSystemWatcher()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AlreadyDisposed == 0)
            {
                // dispose of the managed and unmanaged resources
                Dispose(true);

                // tell the GC that the Finalize process no longer needs
                // to be run for this object.		
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                var disposedAlready = Interlocked.Exchange(ref m_AlreadyDisposed, 1);
                if (disposedAlready != 0)
                {
                    return;
                }

                // Dispose managed resources.

                m_FileSystemWatcher.Changed -= m_FileSystemWatcherChangedEvent;
                m_FileSystemWatcher.Created -= m_FileSystemWatcherCreatedEvent;
                m_FileSystemWatcher.Deleted -= m_FileSystemWatcherDeletedEvent;
                m_FileSystemWatcher.Renamed -= m_FileSystemWatcherRenamedEvent;

                m_FileSystemWatcher.Dispose();

                m_FileCreatedPreviouslyEvent = null;
                m_FileChangedEvent = null;
                m_FileCreatedEvent = null;
                m_FileDeletedEvent = null;
                m_FileRenamedEvent = null;
                m_FileFinishedChangingEvent = null;
            }
            // Dispose unmanaged resources.

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
        }
        #endregion
    }
}
