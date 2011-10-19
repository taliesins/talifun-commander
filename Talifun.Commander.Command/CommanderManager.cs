using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NLog;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Properties;
using Talifun.Commander.FileWatcher;

namespace Talifun.Commander.Command
{
    internal class CommanderManager : ICommanderManager, IDisposable
    {
    	private readonly IEnhancedFileSystemWatcherFactory _enhancedFileSystemWatcherFactory;
    	private readonly CommandConfigurationTester _commandConfigurationTester;

    	private readonly List<IEnhancedFileSystemWatcher> _enhancedFileSystemWatchers = new List<IEnhancedFileSystemWatcher>();
    	private FileFinishedChangingEventHandler _fileFinishedChangingEvent;

    	private bool _stopSignalled = false;

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

            _commandConfigurationTester = new CommandConfigurationTester(_container);

            CheckConfiguration();

            var projects = _commanderSettings.Projects;
            for (var j = 0; j < projects.Count; j++)
            {
                var folderSettings = _commanderSettings.Projects[j].Folders;

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
                                                CommanderManager = this,
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

        #region Test Configuration

    	private void CheckConfiguration()
        {
            var projects = _commanderSettings.Projects;
            for (var j = 0; j < projects.Count; j++)
            {
				_commandConfigurationTester.CheckProjectConfiguration(_appSettings, projects[j]);
            }
        }

        #endregion

        #region ICommanderManager Members

		public CommanderSectionWindow GetCommanderSectionWindow()
		{
			return new CommanderSectionWindow(_container, _appSettings, _commanderSettings);
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
