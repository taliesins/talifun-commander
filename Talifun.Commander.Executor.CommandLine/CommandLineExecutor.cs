using System;
using System.Diagnostics;
using System.Text;

namespace Talifun.Commander.Executor.CommandLine
{
    public class CommandLineExecutor : IExectutor
    {
        protected bool m_UnableToExecuteCommand;
        protected StringBuilder m_Output;

        protected DataReceivedEventHandler m_DataReceivedEventHandlerOutput;
        protected DataReceivedEventHandler m_DataReceivedEventHandlerError;

        public bool Execute(string workingDirectory, string commandPath, string commandArguments, out string output)
        {
            m_UnableToExecuteCommand = false;
            m_Output = new StringBuilder();

            m_DataReceivedEventHandlerOutput = new DataReceivedEventHandler(OnOutputDataReceived);
            m_DataReceivedEventHandlerError = new DataReceivedEventHandler(OnErrorDataReceived);

            var processEncode = new Process();

            try
            {
                processEncode.StartInfo.WorkingDirectory = workingDirectory;
                processEncode.StartInfo.FileName = commandPath;
                processEncode.StartInfo.Arguments = commandArguments;
                processEncode.StartInfo.UseShellExecute = false;
                processEncode.StartInfo.CreateNoWindow = true;
                processEncode.StartInfo.RedirectStandardError = true;
                processEncode.StartInfo.RedirectStandardOutput = true;
                processEncode.OutputDataReceived += m_DataReceivedEventHandlerOutput;
                processEncode.ErrorDataReceived += m_DataReceivedEventHandlerError;

                m_Output.Append(commandPath + " ");
                m_Output.AppendLine(commandArguments);

                processEncode.Start();
                processEncode.BeginOutputReadLine();
                processEncode.BeginErrorReadLine();
                processEncode.WaitForExit();
            }
            finally
            {
                processEncode.OutputDataReceived -= m_DataReceivedEventHandlerOutput;
                processEncode.ErrorDataReceived -= m_DataReceivedEventHandlerError;
                processEncode.Close();

                m_DataReceivedEventHandlerOutput = null;
                m_DataReceivedEventHandlerError = null;
            }

            output = m_Output.ToString();

            return !m_UnableToExecuteCommand;
        }

        protected virtual void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                m_Output.AppendLine(data.Trim());
            }
        }

        protected virtual void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = e.Data;
            if (!String.IsNullOrEmpty(data))
            {
                m_Output.Append(data.Trim() + Environment.NewLine);
                m_UnableToExecuteCommand = true;
            }
        }
    }
}
