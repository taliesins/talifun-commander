using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Magnum;
using MassTransit;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FolderWatcher.Messages;
using Talifun.Commander.FileWatcher;

namespace Talifun.Commander.Command.FolderWatcher
{
	public class FolderWatcherService : IFolderWatcherService, IDisposable
	{
		private readonly IEnhancedFileSystemWatcherFactory _enhancedFileSystemWatcherFactory;
		private readonly List<IEnhancedFileSystemWatcher> _enhancedFileSystemWatchers = new List<IEnhancedFileSystemWatcher>();
		private FileFinishedChangingEventHandler _fileFinishedChangingEvent;

		public FolderWatcherService(IEnhancedFileSystemWatcherFactory enhancedFileSystemWatcherFactory)
		{
			_enhancedFileSystemWatcherFactory = enhancedFileSystemWatcherFactory;
			SetupFolderWatchers();
		}

		public void Start()
		{
			foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
			{
				enhancedFileSystemWatcher.Start();
			}
		}

		public void Stop()
		{
			foreach (var enhancedFileSystemWatcher in _enhancedFileSystemWatchers)
			{
				enhancedFileSystemWatcher.Stop();
			}
		}

		private CommanderSection CommanderSettings
		{
			get { return CurrentConfiguration.CommanderSettings; }
		}

		private void SetupFolderWatchers()
		{
			var projects = CommanderSettings.Projects;
			for (var j = 0; j < projects.Count; j++)
			{
				var folderSettings = CommanderSettings.Projects[j].Folders;

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

			var fileFinishedChangingMessage = new FileFinishedChangingMessage
			{
				CorrelationId = CombGuid.Generate(),
				FilePath = e.FilePath,
				Folder = (FolderElement)e.UserState
			};

			BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName).Publish(fileFinishedChangingMessage);
		}

        #region IDisposable Members
        private int _alreadyDisposed = 0;

		~FolderWatcherService()
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
