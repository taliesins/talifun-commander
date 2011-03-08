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
        protected const string m_EventLog = "TalifunCommanderLog";
        protected const string m_EventLogSource = "TalifunCommanderLogSource";
        private ICommanderManager _mCommanderManager;
        private UnhandledExceptionEventHandler m_UnhandledExceptionEventHandler;
        private CommandErrorEventHandler m_CommandErrorEventHandler;
        public CommanderService()
        {
            m_UnhandledExceptionEventHandler = new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.UnhandledException += m_UnhandledExceptionEventHandler;
            InitializeComponent();

            if (!EventLog.SourceExists(m_EventLogSource))
            {
                EventLog.CreateEventSource(m_EventLogSource, m_EventLog);
            }

            CommanderEventLog.Source = m_EventLogSource;
            CommanderEventLog.Log = m_EventLog;

            _mCommanderManager = CommanderManagerFactory.Instance.CreateCommandManager();
            m_CommandErrorEventHandler = new CommandErrorEventHandler(CommandManager_OnCommandErrorEvent);
            _mCommanderManager.CommandErrorEvent += m_CommandErrorEventHandler;
        }

        protected override void OnStart(string[] args)
        {
            _mCommanderManager.Start();
            CommanderEventLog.WriteEntry("Talifun Commander service started", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            _mCommanderManager.Stop();
            CommanderEventLog.WriteEntry("Talifun Commander service stopped", EventLogEntryType.Information);
        }

        protected void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
