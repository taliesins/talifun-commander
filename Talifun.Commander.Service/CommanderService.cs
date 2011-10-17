﻿using System;
using System.ServiceProcess;
using NLog;
using Talifun.Commander.Command;

namespace Talifun.Commander.Service
{
    public partial class CommanderService : ServiceBase
    {
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICommanderManager _commanderManager;
        private readonly UnhandledExceptionEventHandler _unhandledExceptionEventHandler;
        
        public CommanderService()
        {
            _unhandledExceptionEventHandler = new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += _unhandledExceptionEventHandler;

            InitializeComponent();

            _commanderManager = CommanderManagerFactory.Instance.CreateCommandManager();            
        }

		protected override void OnStart(string[] args)
		{
			_logger.Info("Talifun Commander service starting");
			_commanderManager.Start();
			_logger.Info("Talifun Commander service started");
		}

		protected override void OnStop()
		{
			_logger.Info("Talifun Commander service stopping");
			_commanderManager.Stop();
			_logger.Info("Talifun Commander service stopped");
		}

		protected void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = (Exception)e.ExceptionObject;
			_logger.FatalException("Unhandled exception", exception);
		}
    }
}
