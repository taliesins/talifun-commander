using System;
using System.Diagnostics;

namespace Talifun.Commander.Executor.FFMpeg
{
    public class FfMpegCommandLineExecutor : CommandLine.CommandLineExecutor
    {
        protected override void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                m_Output.Append(e.Data.Trim() + Environment.NewLine);

                if (CheckConversionSuccess(e.Data) > 0)
                {
                    base.m_UnableToExecuteCommand = true;
                }
            }
        }

        protected static int CheckConversionSuccess(string output)
        {
            if (output.IndexOf("Unknown format") >= 0)
                return 1;
            if (output.IndexOf("Unsupported codec") >= 0)
                return 2;
            if (output.IndexOf("could not find codec parameters") >= 0)
                return 3;
            return 0;
        }
    }
}
