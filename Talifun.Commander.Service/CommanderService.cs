using System;
using System.Globalization;
using System.ServiceProcess;
using System.Diagnostics;
using System.Text;
using Talifun.Commander.Command;

namespace Talifun.Commander.Service
{
    public partial class CommanderService : ServiceBase
    {
        protected const string Log = "TalifunCommanderLog";
        protected const string LogSource = "TalifunCommanderLogSource";
        private readonly ICommanderManager _commanderManager;
        private readonly UnhandledExceptionEventHandler _unhandledExceptionEventHandler;
        private readonly CommandErrorEventHandler _commandErrorEventHandler;
        public CommanderService()
        {
            _unhandledExceptionEventHandler = new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += _unhandledExceptionEventHandler;
            InitializeComponent();

            if (!EventLog.SourceExists(LogSource))
            {
                EventLog.CreateEventSource(LogSource, Log);
            }

            CommanderEventLog.Source = LogSource;
            CommanderEventLog.Log = Log;

            _commanderManager = CommanderManagerFactory.Instance.CreateCommandManager();
            _commandErrorEventHandler = new CommandErrorEventHandler(CommandManager_OnCommandErrorEvent);
            _commanderManager.CommandErrorEvent += _commandErrorEventHandler;
        }

        protected override void OnStart(string[] args)
        {
            _commanderManager.Start();
            CommanderEventLog.WriteEntry("Talifun Commander service started", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            _commanderManager.Stop();
            CommanderEventLog.WriteEntry("Talifun Commander service stopped", EventLogEntryType.Information);
        }

        protected void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            CommanderEventLog.WriteEntry(LogException(exception), EventLogEntryType.Error);
        }

        protected void CommandManager_OnCommandErrorEvent(object sender, CommandErrorEventArgs e)
        {
            CommanderEventLog.WriteEntry(e.ExceptionMessage, EventLogEntryType.Error);
        }

        public static string LogException(Exception exception)
        {
            var error = new StringBuilder();
            error.Append("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            error.Append("Computer name:     " + Environment.MachineName + Environment.NewLine);
            error.Append("User name:         " + Environment.UserName + Environment.NewLine);
            error.Append("OS:                " + Environment.OSVersion.ToString() + Environment.NewLine);
            error.Append("Culture:           " + CultureInfo.CurrentCulture.Name + Environment.NewLine);

            error.Append("Exception class:   " + exception.GetType().ToString() + Environment.NewLine);
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

        public static string GetExceptionStack(Exception e)
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
    }
}
