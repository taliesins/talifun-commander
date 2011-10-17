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
        private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);
        private readonly AsyncOperation _asyncOperation = AsyncOperationManager.CreateOperation(null);

        private readonly object _filesRaisingEventsLock = new object();
        private Dictionary<string, IFileChangingItem> _filesChanging;

        private readonly System.Timers.Timer _timer = new System.Timers.Timer();
        private string _nextFileToCheck = string.Empty;

        private readonly FileSystemEventHandler _fileSystemWatcherChangedEvent;
        private readonly FileSystemEventHandler _fileSystemWatcherCreatedEvent;
        private readonly FileSystemEventHandler _fileSystemWatcherDeletedEvent;
        private readonly RenamedEventHandler _fileSystemWatcherRenamedEvent;
        private readonly FileFinishedChangingCallback _fileFinishedChangingCallback;
        private readonly FileSystemWatcher _fileSystemWatcher;

    	public EnhancedFileSystemWatcher(string folderToWatch, string filter, int pollTime, bool includeSubdirectories)
        {
            FolderToWatch = folderToWatch;
            Filter = filter;
            PollTime = pollTime;
            IncludeSubdirectories = includeSubdirectories;

            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimeUp);
            _timer.Interval = PollTime;
            _timer.Enabled = true;
            _timer.AutoReset = false;

            _fileSystemWatcherChangedEvent = new FileSystemEventHandler(OnFileChanged);
            _fileSystemWatcherCreatedEvent = new FileSystemEventHandler(OnFileCreated);
            _fileSystemWatcherDeletedEvent = new FileSystemEventHandler(OnFileDeleted);
            _fileSystemWatcherRenamedEvent = new RenamedEventHandler(OnFileRenamed);
            _fileFinishedChangingCallback = new FileFinishedChangingCallback(OnFileFinishedChanging);

            _fileSystemWatcher = !string.IsNullOrEmpty(Filter) ? new FileSystemWatcher(FolderToWatch, Filter) : new FileSystemWatcher(FolderToWatch);
            _fileSystemWatcher.IncludeSubdirectories = IncludeSubdirectories;
            _fileSystemWatcher.EnableRaisingEvents = false;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            _fileSystemWatcher.Changed += _fileSystemWatcherChangedEvent;
            _fileSystemWatcher.Created += _fileSystemWatcherCreatedEvent;
            _fileSystemWatcher.Deleted += _fileSystemWatcherDeletedEvent;
            _fileSystemWatcher.Renamed += _fileSystemWatcherRenamedEvent;
        }

        public void Start()
        {
            if (_fileSystemWatcher.EnableRaisingEvents) return;
            _filesChanging = new Dictionary<string, IFileChangingItem>();
            _fileSystemWatcher.EnableRaisingEvents = true;

            RaiseEventsForExistingFiles();
        }

        public void Stop()
        {
            if (!_fileSystemWatcher.EnableRaisingEvents) return;
            _fileSystemWatcher.EnableRaisingEvents = false;

            Thread.Sleep(0);

            lock (_filesRaisingEventsLock)
            {
                _timer.Stop();
                _nextFileToCheck = string.Empty;
                _filesChanging.Clear();
            }
        }

    	public object UserState { get; set; }

    	public bool IsRunning
        {
            get
            {
                return _fileSystemWatcher.EnableRaisingEvents;
            }
        }

        public string FolderToWatch { get; private set; }

        public string Filter { get; private set; }

        public int PollTime { get; private set; }

        public bool IncludeSubdirectories { get; private set; }

        private void RaiseEventsForExistingFiles()
        {
            GetAllFilesToCheck(FolderToWatch);
        }

        private void GetAllFilesToCheck(string folderPath)
        {
            if (folderPath == null || folderPath.Length <= 0) return;
            // search in subdirectories
            if (IncludeSubdirectories)
            {
                var folders = Directory.GetDirectories(folderPath);
                foreach (string folder in folders)
                {
                    GetAllFilesToCheck(folder);
                }
            }

            string[] files = null;
            files = !string.IsNullOrEmpty(Filter) ? Directory.GetFiles(folderPath, Filter) : Directory.GetFiles(folderPath);

			foreach (var file in files)
			{
				var fileInfo = new FileInfo(file);
				Push(file, new FileSystemEventArgs(WatcherChangeTypes.All, fileInfo.DirectoryName, fileInfo.Name));
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

            var item = _filesChanging[filePath];
            item.FireTime = DateTime.Now.AddMilliseconds(PollTime);

            if (_nextFileToCheck == filePath)
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
            item.FireTime = DateTime.Now.AddMilliseconds(PollTime);
            _filesChanging.Add(filePath, item);

            if (string.IsNullOrEmpty(_nextFileToCheck))
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

            _filesChanging.Remove(filePath);

            if (_nextFileToCheck == filePath)
            {
                GetNextFileToCheck();
            }
        }

        private void GetNextFileToCheck()
        {
            _timer.Stop();

            var currentDateTime = DateTime.Now;

            _nextFileToCheck = string.Empty;

            if (_filesChanging == null || _filesChanging.Count < 1) return;

            var lowestDateTime = DateTime.MaxValue;
            var nextFileToCheck = string.Empty;
            foreach (var item in _filesChanging)
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

            _nextFileToCheck = nextFileToCheck;
            _timer.Interval = interval;
            _timer.Start();
        }

        #endregion

        #region Events raised by file watcher

        private void OnTimeUp(object sender, ElapsedEventArgs e)
        {
            if (_timer == null) return;
            lock (_filesRaisingEventsLock)
            {
                _timer.Stop();

                if (!string.IsNullOrEmpty(_nextFileToCheck) && _filesChanging.ContainsKey(_nextFileToCheck))
                {
                    var item = _filesChanging[_nextFileToCheck];
                    if (item.FireTime <= DateTime.Now)
                    {
                        _fileFinishedChangingCallback(item.FileSystemEventArgs);
                    }
                }

                GetNextFileToCheck();
            }
        }

        private void OnFileFinishedChanging(FileSystemEventArgs e)
        {
            var filePath = e.FullPath;

            lock (_filesRaisingEventsLock)
            {
                if (_filesChanging.ContainsKey(filePath))
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

            var fileFinishedChangingEventArgs = new FileFinishedChangingEventArgs(filePath, WatcherChangeTypes.Changed, UserState);
            RaiseAsynchronousOnFileFinishedChangingEvent(fileFinishedChangingEventArgs);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;

            if (e.ChangeType != WatcherChangeTypes.Created) return;
            lock (_filesRaisingEventsLock)
            {
                if (_filesChanging.ContainsKey(filePath))
                {
                    Touch(filePath);
                }
                else
                {
                    Push(filePath, e);
                }
            }

            var fileCreatedEvent = new FileCreatedEventArgs(e.FullPath, UserState);
            RaiseAsynchronousOnFileCreatedEvent(fileCreatedEvent);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;

            lock (_filesRaisingEventsLock)
            {
                if (_filesChanging.ContainsKey(filePath))
                {
                    Touch(filePath);
                }
                else
                {
                    Push(filePath, e);
                }
            }

            var fileChangedEventArgs = new FileChangedEventArgs(filePath, UserState);
            RaiseAsynchronousOnFileChangedEvent(fileChangedEventArgs);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;

            lock (_filesRaisingEventsLock)
            {
                if (_filesChanging.ContainsKey(filePath))
                {
                    Pop(filePath);

                    var fileFinishedChangingEventArgs = new FileFinishedChangingEventArgs(filePath, WatcherChangeTypes.Deleted, UserState);
                    RaiseAsynchronousOnFileFinishedChangingEvent(fileFinishedChangingEventArgs);
                }
            }

            var fileDeletedEvent = new FileDeletedEventArgs(e.FullPath, UserState);
            RaiseAsynchronousOnFileDeletedEvent(fileDeletedEvent);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            var filePath = e.FullPath;

            lock (_filesRaisingEventsLock)
            {
                if (_filesChanging.ContainsKey(filePath))
                {
                    Pop(filePath);

                    var fileFinishedChangingEventArgs = new FileFinishedChangingEventArgs(filePath, WatcherChangeTypes.Renamed, UserState);
                    RaiseAsynchronousOnFileFinishedChangingEvent(fileFinishedChangingEventArgs);
                }
            }

            var fileRenamedEvent = new FileRenamedEventArgs(e.OldFullPath, e.FullPath, UserState);
            RaiseAsynchronousOnFileRenamedEvent(fileRenamedEvent);
        }

        #endregion

        

        #region FileChangedEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private FileChangedEventHandler _fileChangedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object _fileChangedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileChangedEventHandler FileChangedEvent
        {
            add
            {
                if (!Monitor.TryEnter(_fileChangedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileChangedEvent.add");
                }
                try
                {
                    _fileChangedEvent += value;
                }
                finally
                {
                    Monitor.Exit(_fileChangedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(_fileChangedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileChangedEvent.remove");
                }
                try
                {
                    _fileChangedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(_fileChangedEventLock);
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
            var e = state as FileChangedEventArgs;
            RaiseOnFileChangedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileChangedEvent(FileChangedEventArgs e)
        {
            _asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileChangedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileChangedEvent(FileChangedEventArgs e)
        {
            _asyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileChangedEventRaised), e);
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

            if (!Monitor.TryEnter(_fileChangedEventLock, _lockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileChangedEvent");
            }
            try
            {
                eventHandler = _fileChangedEvent;
            }
            finally
            {
                Monitor.Exit(_fileChangedEventLock);
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
        private FileCreatedEventHandler _fileCreatedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object _fileCreatedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileCreatedEventHandler FileCreatedEvent
        {
            add
            {
                if (!Monitor.TryEnter(_fileCreatedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileCreatedEvent.add");
                }
                try
                {
                    _fileCreatedEvent += value;
                }
                finally
                {
                    Monitor.Exit(_fileCreatedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(_fileCreatedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileCreatedEvent.remove");
                }
                try
                {
                    _fileCreatedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(_fileCreatedEventLock);
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
            var e = state as FileCreatedEventArgs;
            RaiseOnFileCreatedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileCreatedEvent(FileCreatedEventArgs e)
        {
            _asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileCreatedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileCreatedEvent(FileCreatedEventArgs e)
        {
            _asyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileCreatedEventRaised), e);
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

            if (!Monitor.TryEnter(_fileCreatedEventLock, _lockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileCreatedEvent");
            }
            try
            {
                eventHandler = _fileCreatedEvent;
            }
            finally
            {
                Monitor.Exit(_fileCreatedEventLock);
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
        private FileDeletedEventHandler _fileDeletedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object _fileDeletedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileDeletedEventHandler FileDeletedEvent
        {
            add
            {
                if (!Monitor.TryEnter(_fileDeletedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileDeletedEvent.add");
                }
                try
                {
                    _fileDeletedEvent += value;
                }
                finally
                {
                    Monitor.Exit(_fileDeletedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(_fileDeletedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileDeletedEvent.remove");
                }
                try
                {
                    _fileDeletedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(_fileDeletedEventLock);
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
            var e = state as FileDeletedEventArgs;
            RaiseOnFileDeletedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileDeletedEvent(FileDeletedEventArgs e)
        {
            _asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileDeletedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileDeletedEvent(FileDeletedEventArgs e)
        {
            _asyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileDeletedEventRaised), e);
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

            if (!Monitor.TryEnter(_fileDeletedEventLock, _lockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileDeletedEvent");
            }
            try
            {
                eventHandler = _fileDeletedEvent;
            }
            finally
            {
                Monitor.Exit(_fileDeletedEventLock);
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
        private FileRenamedEventHandler _fileRenamedEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object _fileRenamedEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileRenamedEventHandler FileRenamedEvent
        {
            add
            {
                if (!Monitor.TryEnter(_fileRenamedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileRenamedEvent.add");
                }
                try
                {
                    _fileRenamedEvent += value;
                }
                finally
                {
                    Monitor.Exit(_fileRenamedEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(_fileRenamedEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileRenamedEvent.remove");
                }
                try
                {
                    _fileRenamedEvent -= value;
                }
                finally
                {
                    Monitor.Exit(_fileRenamedEventLock);
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
            var e = state as FileRenamedEventArgs;
            RaiseOnFileRenamedEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileRenamedEvent(FileRenamedEventArgs e)
        {
            _asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileRenamedEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileRenamedEvent(FileRenamedEventArgs e)
        {
            _asyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileRenamedEventRaised), e);
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

            if (!Monitor.TryEnter(_fileRenamedEventLock, _lockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileRenamedEvent");
            }
            try
            {
                eventHandler = _fileRenamedEvent;
            }
            finally
            {
                Monitor.Exit(_fileRenamedEventLock);
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
        private FileFinishedChangingEventHandler _fileFinishedChangingEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object _fileFinishedChangingEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event FileFinishedChangingEventHandler FileFinishedChangingEvent
        {
            add
            {
                if (!Monitor.TryEnter(_fileFinishedChangingEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileFinishedChangingEvent.add");
                }
                try
                {
                    _fileFinishedChangingEvent += value;
                }
                finally
                {
                    Monitor.Exit(_fileFinishedChangingEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(_fileFinishedChangingEventLock, _lockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - FileFinishedChangingEvent.remove");
                }
                try
                {
                    _fileFinishedChangingEvent -= value;
                }
                finally
                {
                    Monitor.Exit(_fileFinishedChangingEventLock);
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
            var e = state as FileFinishedChangingEventArgs;
            RaiseOnFileFinishedChangingEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnFileFinishedChangingEvent(FileFinishedChangingEventArgs e)
        {
            _asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnFileFinishedChangingEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnFileFinishedChangingEvent(FileFinishedChangingEventArgs e)
        {
            _asyncOperation.Post(new SendOrPostCallback(AsynchronousOnFileFinishedChangingEventRaised), e);
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

            if (!Monitor.TryEnter(_fileFinishedChangingEventLock, _lockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnFileFinishedChangingEvent");
            }
            try
            {
                eventHandler = _fileFinishedChangingEvent;
            }
            finally
            {
                Monitor.Exit(_fileFinishedChangingEventLock);
            }

            OnFileFinishedChangingEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region IDisposable Members
        private int _alreadyDisposed = 0;

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
            if (_alreadyDisposed != 0) return;
            // dispose of the managed and unmanaged resources
            Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.		
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposeManagedResources)
        {
            if (!disposeManagedResources) return;
            var disposedAlready = Interlocked.Exchange(ref _alreadyDisposed, 1);
            if (disposedAlready != 0)
            {
                return;
            }

            // Dispose managed resources.

            _fileSystemWatcher.Changed -= _fileSystemWatcherChangedEvent;
            _fileSystemWatcher.Created -= _fileSystemWatcherCreatedEvent;
            _fileSystemWatcher.Deleted -= _fileSystemWatcherDeletedEvent;
            _fileSystemWatcher.Renamed -= _fileSystemWatcherRenamedEvent;

            _fileSystemWatcher.Dispose();

            _fileChangedEvent = null;
            _fileCreatedEvent = null;
            _fileDeletedEvent = null;
            _fileRenamedEvent = null;
            _fileFinishedChangingEvent = null;
            // Dispose unmanaged resources.

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
        }
        #endregion
    }
}
