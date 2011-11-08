using System;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MassTransit;
using NLog;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Properties;
using Talifun.Commander.Command.TestConfiguration;
using Talifun.Commander.FileWatcher;

namespace Talifun.Commander.Command
{
	internal class CommanderManager : ICommanderManager, IDisposable
    {
		private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);
		private readonly AsyncOperation _asyncOperation = AsyncOperationManager.CreateOperation(null);

    	private readonly IEnhancedFileSystemWatcherFactory _enhancedFileSystemWatcherFactory;
    	private readonly List<IEnhancedFileSystemWatcher> _enhancedFileSystemWatchers = new List<IEnhancedFileSystemWatcher>();
    	private readonly ICommandManagerServiceBuses _commandManagerServiceBuses = new CommandManagerServiceBuses();
		
		private FileFinishedChangingEventHandler _fileFinishedChangingEvent;

    	private bool _startOrStopSignalled = false;

    	private readonly ExportProvider _container;
		private readonly AppSettingsSection _appSettings;
    	private readonly CommanderSection _commanderSettings;

		public CommanderManager(ExportProvider container, AppSettingsSection appSettings, CommanderSection commanderSettings, IEnhancedFileSystemWatcherFactory enhancedFileSystemWatcherFactory)
        {
            _container = container;
			_appSettings = appSettings;
            _commanderSettings = commanderSettings;
            _enhancedFileSystemWatcherFactory = enhancedFileSystemWatcherFactory;

            IsRunning = false;

            var projects = _commanderSettings.Projects;
            for (var j = 0; j < projects.Count; j++)
            {
                var folderSettings = _commanderSettings.Projects[j].Folders;

                for (var i = 0; i < folderSettings.Count; i++)
                {
                    var folderSetting = folderSettings[i];
                    var enhancedFileSystemWatcher =
                        _enhancedFileSystemWatcherFactory.CreateEnhancedFileSystemWatcher(
                            folderSetting.GetFolderToWatchOrDefault(), folderSetting.Filter, folderSetting.PollTime,
                            folderSetting.IncludeSubdirectories, folderSetting);
                    _enhancedFileSystemWatchers.Add(enhancedFileSystemWatcher);
                }
            }

            _fileFinishedChangingEvent = new FileFinishedChangingEventHandler(OnFileFinishedChangingEvent);

            foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.FileFinishedChangingEvent += _fileFinishedChangingEvent;
            }
        }

    	private void OnFileFinishedChangingEvent(object sender, FileFinishedChangingEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted || e.ChangeType == WatcherChangeTypes.Renamed) return;
            var folderSetting = (FolderElement)e.UserState;
            ProcessFileMatches(e.FilePath, folderSetting);
        }

    	private void ProcessFileMatches(string filePath, FolderElement folderSetting)
        {
			var fileInfo = new FileInfo(filePath);
			if (!fileInfo.Exists)
			{
				return;
			}

			fileInfo.WaitForFileToUnlock(10, 500);

            var fileName = fileInfo.Name;

            const RegexOptions regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;
            var fileMatchSettings = folderSetting.FileMatches;

            var uniqueDirectoryName = "master." + fileName + "." + Guid.NewGuid();

    		var workingPath = folderSetting.GetWorkingPathOrDefault();
			var workingDirectoryPath = !string.IsNullOrEmpty(workingPath) ?
				new DirectoryInfo(Path.Combine(workingPath, uniqueDirectoryName)) 
                : new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));

            var workingFilePath = new FileInfo(Path.Combine(workingDirectoryPath.FullName, fileName));
            try
            {
                workingDirectoryPath.Create();

				fileInfo.WaitForFileToUnlock(10, 500);
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
					workingFilePath.WaitForFileToUnlock(10, 500);
                    workingFilePath.Refresh();
                }
            }
            finally
            {
            	var completedPath = folderSetting.GetCompletedPathOrDefault();
				if (!string.IsNullOrEmpty(completedPath) && workingFilePath.Exists)
                {
					var completedFilePath = new FileInfo(Path.Combine(completedPath, fileName));
                    if (completedFilePath.Exists)
                    {
                        completedFilePath.Delete();
                    }

                    //Make sure that processing on file has stopped
					workingFilePath.WaitForFileToUnlock(10, 500);
                    workingFilePath.Refresh();
                    workingFilePath.MoveTo(completedFilePath.FullName);
                }

                if (workingDirectoryPath.Exists)
                {
                    workingDirectoryPath.Delete(true);
                }
            }
        }

    	private ProjectElement GetCurrentProject(FileMatchElement fileMatch)
        {
            var projects = _commanderSettings.Projects;

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
                                                FileMatch = fileMatch,
                                                InputFilePath = fileInfo,
                                                Project = project,
												AppSettings = _appSettings
                                            };

			var logger = LogManager.GetLogger(commandSaga.Settings.ElementType.FullName);
			logger.Info(string.Format(Resource.InfoMessageSagaStarted, project.Name, fileMatch.Name, fileInfo));
			commandSaga.Run(commandSagaProperties);
			logger.Info(string.Format(Resource.InfoMessageSagaCompleted, project.Name, fileMatch.Name, fileInfo));
        }

        #region ICommanderManager Members

		public CommanderSectionWindow GetCommanderSectionWindow()
		{
			return new CommanderSectionWindow(_container, _appSettings, _commanderSettings);
		}

		public void Start()
        {
            if (IsRunning || _startOrStopSignalled) return;
			_startOrStopSignalled = true;
        	_commandManagerServiceBuses.Start();

			var bus = BusDriver.Instance.GetBus(CommandManagerServiceBuses.CommandManagerBusName);
			bus.SubscribeHandler<ResponseTestConfigurationMessage>((message) =>
			{
				IsRunning = true;
				StartEnhancedFileSystemWatchers();
				_startOrStopSignalled = false;

				var commanderStartedEventArgs = new CommanderStartedEventArgs();

				RaiseAsynchronousOnCommanderStartedEvent(commanderStartedEventArgs);
			});

			var requestTestConfigurationMessage = new RequestTestConfigurationMessage()
			{
			};

			bus.Publish(requestTestConfigurationMessage);
        }

        public void Stop()
        {
            if (!IsRunning || _startOrStopSignalled) return;
            _startOrStopSignalled = true;
            StopEnhancedFileSystemWatchers();
			_commandManagerServiceBuses.Stop();
            IsRunning = false;
            _startOrStopSignalled = false;

			var commanderStoppedEventArgs = new CommanderStoppedEventArgs();
			RaiseAsynchronousOnCommanderStoppedEvent(commanderStoppedEventArgs);
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

		#region CommanderStartedEvent
		/// <summary>
		/// Where the actual event is stored.
		/// </summary>
		private CommanderStartedEventHandler _commanderStartedEvent;

		/// <summary>
		/// Lock for event delegate access.
		/// </summary>
		private readonly object _commanderStartedEventLock = new object();

		/// <summary>
		/// The event that is fired.
		/// </summary>
		public event CommanderStartedEventHandler CommanderStartedEvent
		{
			add
			{
				if (!Monitor.TryEnter(_commanderStartedEventLock, _lockTimeout))
				{
					throw new ApplicationException("Timeout waiting for lock - CommanderStartedEvent.add");
				}
				try
				{
					_commanderStartedEvent += value;
				}
				finally
				{
					Monitor.Exit(_commanderStartedEventLock);
				}
			}
			remove
			{
				if (!Monitor.TryEnter(_commanderStartedEventLock, _lockTimeout))
				{
					throw new ApplicationException("Timeout waiting for lock - CommanderStartedEvent.remove");
				}
				try
				{
					_commanderStartedEvent -= value;
				}
				finally
				{
					Monitor.Exit(_commanderStartedEventLock);
				}
			}
		}

		/// <summary>
		/// Template method to add default behaviour for the event
		/// </summary>
		private void OnCommanderStartedEvent(CommanderStartedEventArgs e)
		{
			// TODO: Implement default behaviour of OnCommanderStartedEvent
		}

		private void AsynchronousOnCommanderStartedEventRaised(object state)
		{
			var e = state as CommanderStartedEventArgs;
			RaiseOnCommanderStartedEvent(e);
		}

		/// <summary>
		/// Will raise the event on the calling thread synchronously. 
		/// i.e. it will wait until all event handlers have processed the event.
		/// </summary>
		/// <param name="state">The state to be passed to the event.</param>
		private void RaiseCrossThreadOnCommanderStartedEvent(CommanderStartedEventArgs e)
		{
			_asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnCommanderStartedEventRaised), e);
		}

		/// <summary>
		/// Will raise the event on the calling thread asynchronously. 
		/// i.e. it will immediatly continue processing even though event 
		/// handlers have not processed the event yet.
		/// </summary>
		/// <param name="state">The state to be passed to the event.</param>
		private void RaiseAsynchronousOnCommanderStartedEvent(CommanderStartedEventArgs e)
		{
			_asyncOperation.Post(new SendOrPostCallback(AsynchronousOnCommanderStartedEventRaised), e);
		}

		/// <summary>
		/// Will raise the event on the current thread synchronously.
		/// i.e. it will wait until all event handlers have processed the event.
		/// </summary>
		/// <param name="e">The state to be passed to the event.</param>
		private void RaiseOnCommanderStartedEvent(CommanderStartedEventArgs e)
		{
			// Make a temporary copy of the event to avoid possibility of
			// a race condition if the last subscriber unsubscribes
			// immediately after the null check and before the event is raised.

			CommanderStartedEventHandler eventHandler;

			if (!Monitor.TryEnter(_commanderStartedEventLock, _lockTimeout))
			{
				throw new ApplicationException("Timeout waiting for lock - RaiseOnCommanderStartedEvent");
			}
			try
			{
				eventHandler = _commanderStartedEvent;
			}
			finally
			{
				Monitor.Exit(_commanderStartedEventLock);
			}

			OnCommanderStartedEvent(e);

			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}
		#endregion

		#region CommanderStoppedEvent
		/// <summary>
		/// Where the actual event is stored.
		/// </summary>
		private CommanderStoppedEventHandler _commanderStoppedEvent;

		/// <summary>
		/// Lock for event delegate access.
		/// </summary>
		private readonly object _commanderStoppedEventLock = new object();

		/// <summary>
		/// The event that is fired.
		/// </summary>
		public event CommanderStoppedEventHandler CommanderStoppedEvent
		{
			add
			{
				if (!Monitor.TryEnter(_commanderStoppedEventLock, _lockTimeout))
				{
					throw new ApplicationException("Timeout waiting for lock - CommanderStoppedEvent.add");
				}
				try
				{
					_commanderStoppedEvent += value;
				}
				finally
				{
					Monitor.Exit(_commanderStoppedEventLock);
				}
			}
			remove
			{
				if (!Monitor.TryEnter(_commanderStoppedEventLock, _lockTimeout))
				{
					throw new ApplicationException("Timeout waiting for lock - CommanderStoppedEvent.remove");
				}
				try
				{
					_commanderStoppedEvent -= value;
				}
				finally
				{
					Monitor.Exit(_commanderStoppedEventLock);
				}
			}
		}

		/// <summary>
		/// Template method to add default behaviour for the event
		/// </summary>
		private void OnCommanderStoppedEvent(CommanderStoppedEventArgs e)
		{
			// TODO: Implement default behaviour of OnCommanderStoppedEvent
		}

		private void AsynchronousOnCommanderStoppedEventRaised(object state)
		{
			var e = state as CommanderStoppedEventArgs;
			RaiseOnCommanderStoppedEvent(e);
		}

		/// <summary>
		/// Will raise the event on the calling thread synchronously. 
		/// i.e. it will wait until all event handlers have processed the event.
		/// </summary>
		/// <param name="state">The state to be passed to the event.</param>
		private void RaiseCrossThreadOnCommanderStoppedEvent(CommanderStoppedEventArgs e)
		{
			_asyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnCommanderStoppedEventRaised), e);
		}

		/// <summary>
		/// Will raise the event on the calling thread asynchronously. 
		/// i.e. it will immediatly continue processing even though event 
		/// handlers have not processed the event yet.
		/// </summary>
		/// <param name="state">The state to be passed to the event.</param>
		private void RaiseAsynchronousOnCommanderStoppedEvent(CommanderStoppedEventArgs e)
		{
			_asyncOperation.Post(new SendOrPostCallback(AsynchronousOnCommanderStoppedEventRaised), e);
		}

		/// <summary>
		/// Will raise the event on the current thread synchronously.
		/// i.e. it will wait until all event handlers have processed the event.
		/// </summary>
		/// <param name="e">The state to be passed to the event.</param>
		private void RaiseOnCommanderStoppedEvent(CommanderStoppedEventArgs e)
		{
			// Make a temporary copy of the event to avoid possibility of
			// a race condition if the last subscriber unsubscribes
			// immediately after the null check and before the event is raised.

			CommanderStoppedEventHandler eventHandler;

			if (!Monitor.TryEnter(_commanderStoppedEventLock, _lockTimeout))
			{
				throw new ApplicationException("Timeout waiting for lock - RaiseOnCommanderStoppedEvent");
			}
			try
			{
				eventHandler = _commanderStoppedEvent;
			}
			finally
			{
				Monitor.Exit(_commanderStoppedEventLock);
			}

			OnCommanderStoppedEvent(e);

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
                enhancedFileSystemWatcher.FileFinishedChangingEvent -= _fileFinishedChangingEvent;
            }

            foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.Dispose();
            }

            _fileFinishedChangingEvent = null;

            // Dispose unmanaged resources.

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
        }
        #endregion
    }
}
