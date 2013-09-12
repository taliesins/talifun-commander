using System;
using System.Diagnostics;
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
            var stopWatch = new Stopwatch();
			_logger.Info("Talifun Commander service starting");
            stopWatch.Start();
			_commanderManager.Start();
            stopWatch.Stop();
			_logger.Info("Talifun Commander service started (took " + stopWatch.ElapsedMilliseconds + " ms)");
		}

		protected override void OnStop()
		{
            var stopWatch = new Stopwatch();
			_logger.Info("Talifun Commander service stopping");
            stopWatch.Start();
			_commanderManager.Stop();
            stopWatch.Stop();
            _logger.Info("Talifun Commander service stopped (took " + stopWatch.ElapsedMilliseconds + " ms)");
		}

		protected void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = (Exception)e.ExceptionObject;
			HandleUnhandledException(exception);
		}

		private void HandleUnhandledException(Exception exception)
		{
			var message = "Unhandled exception";
			try
			{
				var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
				message = string.Format("Unhandled exception in {0} v{1} - {2}", assemblyName.Name, assemblyName.Version, exception);
			}
			catch (Exception exc)
			{
				_logger.ErrorException("Exception in unhandled exception handler", exc);
			}
			finally
			{
				_logger.ErrorException(message, exception);
			}
		}
    }
}
