using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Properties;
using Talifun.Commander.FileWatcher;

namespace Talifun.Commander.Command
{
    internal class CommanderManager : ICommanderManager, IDisposable
    {
    	private readonly IEnhancedFileSystemWatcherFactory _enhancedFileSystemWatcherFactory;
    	private readonly CommandConfigurationTester _commandConfigurationTester;
    	private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);
    	private readonly AsyncOperation _asyncOperation = AsyncOperationManager.CreateOperation(null);

    	private readonly List<IEnhancedFileSystemWatcher> _enhancedFileSystemWatchers = new List<IEnhancedFileSystemWatcher>();
    	private FileCreatedPreviouslyEventHandler _fileCreatedPreviouslyEvent;
    	private FileFinishedChangingEventHandler _fileFinishedChangingEvent;

    	private bool _stopSignalled = false;

    	private readonly ExportProvider _container;
    	private readonly CommanderSection _configuration;
    	private readonly NameValueCollection _appSettings;

        public CommanderManager(ExportProvider container, CommanderSection configuration, NameValueCollection appSettings, IEnhancedFileSystemWatcherFactory enhancedFileSystemWatcherFactory)
        {
            _container = container;
            _configuration = configuration;
        	_appSettings = appSettings;
            _enhancedFileSystemWatcherFactory = enhancedFileSystemWatcherFactory;

            IsRunning = false;

            _commandConfigurationTester = new CommandConfigurationTester(_container);

            CheckConfiguration();

            var projects = _configuration.Projects;
            for (var j = 0; j < projects.Count; j++)
            {
                var folderSettings = _configuration.Projects[j].Folders;

                for (var i = 0; i < folderSettings.Count; i++)
                {
                    var folderSetting = folderSettings[i];
                    var enhancedFileSystemWatcher =
                        _enhancedFileSystemWatcherFactory.CreateEnhancedFileSystemWatcher(
                            folderSetting.FolderToWatch, folderSetting.Filter, folderSetting.PollTime,
                            folderSetting.IncludeSubdirectories, folderSetting);
                    _enhancedFileSystemWatchers.Add(enhancedFileSystemWatcher);
                }
            }

            _fileCreatedPreviouslyEvent = new FileCreatedPreviouslyEventHandler(OnFileCreatedPreviouslyEvent);
            _fileFinishedChangingEvent = new FileFinishedChangingEventHandler(OnFileFinishedChangingEvent);

            foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.FileCreatedPreviouslyEvent += _fileCreatedPreviouslyEvent;
                enhancedFileSystemWatcher.FileFinishedChangingEvent += _fileFinishedChangingEvent;
            }
        }

    	private void OnFileFinishedChangingEvent(object sender, FileFinishedChangingEventArgs e)
        {
            if (!(e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)) return;
            var folderSetting = (FolderElement)e.UserState;
            ProcessFileMatches(e.FilePath, folderSetting);
        }

    	private void OnFileCreatedPreviouslyEvent(object sender, FileCreatedPreviouslyEventArgs e)
        {
            var folderSetting = (FolderElement)e.UserState;
            ProcessFileMatches(e.FilePath, folderSetting);
        }

    	private void ProcessFileMatches(string filePath, FolderElement folderSetting)
        {
            WaitForFileToUnlock(filePath, 10, 500);
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return;
            }

            var fileName = fileInfo.Name;

            const RegexOptions regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;
            var fileMatchSettings = folderSetting.FileMatches;

            var uniqueDirectoryName = "master." + fileName + "." + Guid.NewGuid();

            var workingDirectoryPath = !string.IsNullOrEmpty(folderSetting.WorkingPath) ? 
                new DirectoryInfo(Path.Combine(folderSetting.WorkingPath, uniqueDirectoryName)) 
                : new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));

            var workingFilePath = new FileInfo(Path.Combine(workingDirectoryPath.FullName, fileName));
            try
            {
                workingDirectoryPath.Create();

                WaitForFileToUnlock(fileInfo.FullName, 10, 500);
                fileInfo.Refresh();
                fileInfo.MoveTo(workingFilePath.FullName);

                for (var i = 0; i < fileMatchSettings.Count; i++)
                {
                    var fileMatch = fileMatchSettings[i];

                    var fileNameMatched = true;
                    if (!string.IsNullOrEmpty(fileMatch.Expression))
                    {
                        fileNameMatched = Regex.IsMatch(fileName, fileMatch.Expression, regxOptions);
                    }

                    if (!fileNameMatched) continue;

                    ProcessFileMatch(workingFilePath, fileMatch);

                    //If the file no longer exists, it assumed that there should be no more processing
                    //e.g. anti-virus may delete file so, we will do no more processing
                    //e.g. video process was unable to process file so it was moved to error processing folder
                    if (!workingFilePath.Exists || fileMatch.StopProcessing)
                    {
                        break;
                    }

                    //Make sure that processing on file has stopped
                    WaitForFileToUnlock(workingFilePath.FullName, 10, 500);
                    workingFilePath.Refresh();
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(folderSetting.CompletedPath) && workingFilePath.Exists)
                {
                    var completedFilePath = new FileInfo(Path.Combine(folderSetting.CompletedPath, fileName));
                    if (completedFilePath.Exists)
                    {
                        completedFilePath.Delete();
                    }

                    //Make sure that processing on file has stopped
                    WaitForFileToUnlock(workingFilePath.FullName, 10, 500);
                    workingFilePath.Refresh();
                    workingFilePath.MoveTo(completedFilePath.FullName);
                }

                if (workingDirectoryPath.Exists)
                {
                    workingDirectoryPath.Delete(true);
                }
            }
        }

    	private static bool WaitForFileToUnlock(string filePath, int retry, int delay)
        {
            for (var i = 0; i < retry; i++)
            {
                if (!IsFileLocked(filePath))
                {
                    return true;
                }
                Thread.Sleep(delay);
            }

            return false;
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

    	private ProjectElement GetCurrentProject(FileMatchElement fileMatch)
        {
            var projects = _configuration.Projects;

            for (var i = 0; i < projects.Count; i++)
            {
                var folders = projects[i].Folders;

                for (var j = 0; j < folders.Count; j++)
                {
                    var fileMatches = folders[j].FileMatches;

                    for (var k = 0; k < fileMatches.Count; k++)
                    {
                        if (fileMatches[k] == fileMatch) return projects[i];
                    }
                }
            }

            throw new Exception(string.Format(Resource.ErrorMessageCannotFindProjectForFileElement));
        }

    	private ICommandSaga GetCommandSaga(string conversionType)
        {
            var commandRunner = _container.GetExportedValues<ICommandSaga>()
                .Where(x => x.Settings.ConversionType == conversionType)
                .First();

            return commandRunner;
        }

    	private void ProcessFileMatch(FileInfo fileInfo, FileMatchElement fileMatch)
        {
            var project = GetCurrentProject(fileMatch);
            var commandSaga = GetCommandSaga(fileMatch.ConversionType);
            var commandSagaProperties = new CommandSagaProperties
                                            {
                                                CommanderManager = this,
                                                FileMatch = fileMatch,
                                                InputFilePath = fileInfo,
                                                Project = project
                                            };
            commandSaga.Run(commandSagaProperties);
        }

        public void LogException(FileInfo errorFileInfo, Exception exception)
        {
            var exceptionMessage = GetExceptionMessage(exception);

            var commandErrorEventArgs = new CommandErrorEventArgs(exceptionMessage);

            RaiseAsynchronousOnCommandErrorEvent(commandErrorEventArgs);

            if (errorFileInfo == null) return;

            using (var streamWriter = errorFileInfo.CreateText())
            {
                streamWriter.Write(exceptionMessage);
            }
        }

    	private static string GetExceptionMessage(Exception exception)
        {
            var error = new StringBuilder();
            error.Append("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            error.Append("Computer name:     " + Environment.MachineName + Environment.NewLine);
            error.Append("User name:         " + Environment.UserName + Environment.NewLine);
            error.Append("OS:                " + Environment.OSVersion.ToString() + Environment.NewLine);
            error.Append("Culture:           " + CultureInfo.CurrentCulture.Name + Environment.NewLine);

            error.Append("Exception class:   " +
            exception.GetType().ToString() + Environment.NewLine);
            error.Append("Exception message: " + GetExceptionStack(exception) + Environment.NewLine);
            error.Append(Environment.NewLine);
            error.Append("Stack Trace:");
            error.Append(Environment.NewLine);
            error.Append(exception.StackTrace);
            error.Append(Environment.NewLine);
            error.Append(Environment.NewLine);
            error.Append("Loaded Modules:");
            error.Append(Environment.NewLine);
            var thisProcess = Process.GetCurrentProcess();
            foreach (ProcessModule module in thisProcess.Modules)
            {
                error.Append(module.FileName + " " + module.FileVersionInfo.FileVersion);
                error.Append(Environment.NewLine);
            }
            error.Append(Environment.NewLine);
            error.Append(Environment.NewLine);
            error.Append(Environment.NewLine);

            return error.ToString();
        }

    	private static string GetExceptionStack(Exception e)
        {
            var message = new StringBuilder();
            message.Append(e.Message);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                message.Append(Environment.NewLine);
                message.Append(e.Message);
            }

            return message.ToString();
        }

        #region Test Configuration

    	private void CheckConfiguration()
        {
            var projects = _configuration.Projects;
            for (var j = 0; j < projects.Count; j++)
            {
				_commandConfigurationTester.CheckProjectConfiguration(projects[j], _appSettings);
            }
        }

        #endregion

        #region ICommanderManager Members

		public CommanderSectionWindow GetCommanderSectionWindow()
		{
			return new CommanderSectionWindow(_container, _configuration);
		}

        public void Start()
        {
            if (IsRunning || _stopSignalled) return;
            IsRunning = true;
            StartEnhancedFileSystemWatchers();
        }

        public void Stop()
        {
            if (!IsRunning || _stopSignalled) return;
            _stopSignalled = true;
            StopEnhancedFileSystemWatchers();
            IsRunning = false;
            _stopSignalled = false;
        }

        public bool IsRunning { get; private set; }

        private void StartEnhancedFileSystemWatchers()
        {
            foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.Start();
            }
        }

        private void StopEnhancedFileSystemWatchers()
        {
            foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.Stop();
            }
        }

        #endregion

        #region CommandErrorEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private CommandErrorEventHandler _commandErrorEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object _commandErrorEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event CommandErrorEventHandler CommandErrorEvent
        {
            add
            {
                if (!Monitor.TryEnter(_commandErrorEventLock, _lockTimeout))
                {
                    throw new ApplicationException(Resource.ErrorMessageRaiseOnCommandErrorEventTimeoutWaitingForLockAdd);
                }
                try
                {
                    _commandErrorEvent += value;
                }
                finally
                {
                    Monitor.Exit(_commandErrorEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(_commandErrorEventLock, _lockTimeout))
                {
                    throw new ApplicationException(Resource.ErrorMessageRaiseOnCommandErrorEventTimeoutWaitingForLockRemove);
                }
                try
                {
                    _commandErrorEvent -= value;
                }
                finally
                {
                    Monitor.Exit(_commandErrorEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        private void OnCommandErrorEvent(CommandErrorEventArgs e)
        {
            // TODO: Implement default behaviour of OnCommandErrorEvent
        }

        private void AsynchronousOnCommandErrorEventRaised(object state)
        {
            var e = state as CommandErrorEventArgs;
            RaiseOnCommandErrorEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnCommandErrorEvent(CommandErrorEventArgs e)
        {
            _asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnCommandErrorEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnCommandErrorEvent(CommandErrorEventArgs e)
        {
            _asyncOperation.Post(new SendOrPostCallback(AsynchronousOnCommandErrorEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnCommandErrorEvent(CommandErrorEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            CommandErrorEventHandler eventHandler;

            if (!Monitor.TryEnter(_commandErrorEventLock, _lockTimeout))
            {
                throw new ApplicationException(Resource.ErrorMessageRaiseOnCommandErrorEventTimeoutWaitingForLock);
            }
            try
            {
                eventHandler = _commandErrorEvent;
            }
            finally
            {
                Monitor.Exit(_commandErrorEventLock);
            }

            OnCommandErrorEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region IDisposable Members
        private int _alreadyDisposed = 0;

        ~CommanderManager()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

    	private void Dispose()
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

            foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.FileCreatedPreviouslyEvent -= _fileCreatedPreviouslyEvent;
                enhancedFileSystemWatcher.FileFinishedChangingEvent -= _fileFinishedChangingEvent;
            }

            foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.Dispose();
            }

            _fileCreatedPreviouslyEvent = null;
            _fileFinishedChangingEvent = null;

            _commandErrorEvent = null;
            // Dispose unmanaged resources.

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
        }
        #endregion
	}
}
