using System;
using System.Windows;
using NLog;

namespace Talifun.Commander.Configurator
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// UI Exceptions
			this.DispatcherUnhandledException += Application_DispatcherUnhandledException;

			// Thread exceptions
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			var exception = e.Exception;
			HandleUnhandledException(exception);
		}

		private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
		{
			HandleUnhandledException(unhandledExceptionEventArgs.ExceptionObject as Exception);
			if (unhandledExceptionEventArgs.IsTerminating)
			{
				_logger.Info("Application is terminating due to an unhandled exception in a secondary thread.");
			}
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
