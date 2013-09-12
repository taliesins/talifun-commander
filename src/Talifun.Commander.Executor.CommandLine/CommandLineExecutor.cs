using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using NLog;
using Talifun.Commander.Command;

namespace Talifun.Commander.Executor.CommandLine
{
    public class CommandLineExecutor : IExectutor
    {
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected bool UnableToExecuteCommand;
        protected StringBuilder Output;

        protected DataReceivedEventHandler DataReceivedEventHandlerOutput;
        protected DataReceivedEventHandler DataReceivedEventHandlerError;

        public bool Execute(CancellationToken cancellationToken, string workingDirectory, string commandPath, string commandArguments, out string output)
        {
            UnableToExecuteCommand = false;
            Output = new StringBuilder();

            DataReceivedEventHandlerOutput = new DataReceivedEventHandler(OnOutputDataReceived);
            DataReceivedEventHandlerError = new DataReceivedEventHandler(OnErrorDataReceived);

            var processCommand = new Process();

            try
            {
                processCommand.StartInfo.WorkingDirectory = workingDirectory;
                processCommand.StartInfo.FileName = commandPath;
                processCommand.StartInfo.Arguments = commandArguments;
                processCommand.StartInfo.UseShellExecute = false;
                processCommand.StartInfo.CreateNoWindow = true;
                processCommand.StartInfo.RedirectStandardError = true;
                processCommand.StartInfo.RedirectStandardOutput = true;
                processCommand.OutputDataReceived += DataReceivedEventHandlerOutput;
                processCommand.ErrorDataReceived += DataReceivedEventHandlerError;

                Output.Append(commandPath + " ");
                Output.AppendLine(commandArguments);

				_logger.Info(string.Format(Properties.Resource.InfoMessageCommandLineStarted, commandPath, commandArguments));

				if (!cancellationToken.IsCancellationRequested)
				{
					cancellationToken.Register(() => processCommand.Kill());
					processCommand.Start();
					processCommand.BeginOutputReadLine();
					processCommand.BeginErrorReadLine();
					processCommand.WaitForExit();
				}
				cancellationToken.ThrowIfCancellationRequested();

            	_logger.Info(string.Format(Properties.Resource.InfoMessageCommandLineCompleted, commandPath, commandArguments));
            }
            finally
            {
                processCommand.OutputDataReceived -= DataReceivedEventHandlerOutput;
                processCommand.ErrorDataReceived -= DataReceivedEventHandlerError;
                processCommand.Close();

                DataReceivedEventHandlerOutput = null;
                DataReceivedEventHandlerError = null;
            }

            output = Output.ToString();

            return !UnableToExecuteCommand;
        }

        protected virtual void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                Output.AppendLine(data.Trim());
            }
        }

        protected virtual void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                Output.Append(data.Trim() + Environment.NewLine);
                UnableToExecuteCommand = true;
            }
        }
    }
}
