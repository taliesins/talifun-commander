using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.Video.WorkFlow
{
	public class FlvWorkFlow : ICommand<IContainerSettings>
    {
		public bool Run(IContainerSettings settings, IDictionary<string, string> appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
			var result = new OnePassWorkFlow().Run(settings, appSettings, inputFilePath, outputDirectoryPath, out outPutFilePath, out output);
            if (result)
            {
				var workingDirectory = outputDirectoryPath.FullName;
				var flvTool2CommandArguments = string.Format("-U \"{0}\"", outPutFilePath.Name);
				var flvTool2CommandPath = appSettings[VideoConversionConfiguration.Instance.FlvTool2PathSettingName];
				var flvTool2Output = string.Empty;

                var commandLineExecutor = new CommandLineExecutor();
                result = commandLineExecutor.Execute(workingDirectory, flvTool2CommandPath, flvTool2CommandArguments, out flvTool2Output);
                output += Environment.NewLine + flvTool2Output;
            }

            return result;
        }
    }
}
