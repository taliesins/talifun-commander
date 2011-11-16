using System;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Threading;
using MassTransit;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker.Request;
using Talifun.Commander.Command.ConfigurationChecker.Response;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FolderWatcher;
using Talifun.Commander.FileWatcher;
using log4net.Config;

namespace Talifun.Commander.Command
{
	internal class CommanderManager : ICommanderManager
    {
		private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);
		private readonly AsyncOperation _asyncOperation = AsyncOperationManager.CreateOperation(null);
    	private readonly ExportProvider _container;
		private readonly AppSettingsSection _appSettings;
    	private readonly CommanderSection _commanderSettings;
		private readonly CommanderService _commanderService = new CommanderService();
		private readonly IFolderWatcherService _folderWatcherService;
		private bool _startOrStopSignalled = false;
		
		public CommanderManager(ExportProvider container, AppSettingsSection appSettings, CommanderSection commanderSettings, IEnhancedFileSystemWatcherFactory enhancedFileSystemWatcherFactory)
        {
			XmlConfigurator.Configure(new FileInfo("log4net.config"));

            _container = container;
			_appSettings = appSettings;
            _commanderSettings = commanderSettings;
			_folderWatcherService = new FolderWatcherService(enhancedFileSystemWatcherFactory);
			IsRunning = false;
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
        	_commanderService.Start();

			var bus = BusDriver.Instance.GetBus(CommanderService.CommandManagerBusName);
			bus.SubscribeHandler<ResponseTestConfigurationMessage>((message) =>
			{
				IsRunning = true;
				_folderWatcherService.Start();
				_startOrStopSignalled = false;

				var commanderStartedEventArgs = new CommanderStartedEventArgs();

				RaiseAsynchronousOnCommanderStartedEvent(commanderStartedEventArgs);
			});

			var requestTestConfigurationMessage = new RequestTestConfigurationMessage();
			bus.Publish(requestTestConfigurationMessage);
        }

        public void Stop()
        {
            if (!IsRunning || _startOrStopSignalled) return;
            _startOrStopSignalled = true;
			_folderWatcherService.Stop();
			_commanderService.Stop();
            IsRunning = false;
            _startOrStopSignalled = false;

			var commanderStoppedEventArgs = new CommanderStoppedEventArgs();
			RaiseAsynchronousOnCommanderStoppedEvent(commanderStoppedEventArgs);
        }

        public bool IsRunning { get; private set; }

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
    }
}
