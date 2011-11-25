using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.Video.Containers;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.Video.WorkFlow
{
	public class Mp4WorkFlow : ICommand<IContainerSettings>
	{
		public bool Run(IContainerSettings settings, Dictionary<string, string> appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
		{
			var result = new TwoPassWorkFlow().Run(settings, appSettings, inputFilePath, outputDirectoryPath, out outPutFilePath, out output);
			if (result && !outPutFilePath.Exists)
			{
				var tempPath = outPutFilePath.FullName;
				var tempFilePath = new FileInfo(outPutFilePath.FullName + "." + Guid.NewGuid().ToString());
				if (tempFilePath.Exists)
				{
					tempFilePath.Delete();
				}
				
				outPutFilePath.MoveTo(tempFilePath.FullName);
				outPutFilePath = new FileInfo(tempPath);

				var workingDirectory = outputDirectoryPath.FullName;
				var qtFastStartCommandArguments = string.Format("\"{0}\" \"{1}\"", tempFilePath.Name, outPutFilePath.Name);
				var qtFastStartCommandPath = appSettings[VideoConversionConfiguration.Instance.QtFastStartPathSettingName];
				var qtFastStartOutput = string.Empty;

				var commandLineExecutor = new CommandLineExecutor();
				result = commandLineExecutor.Execute(workingDirectory, qtFastStartCommandPath, qtFastStartCommandArguments, out qtFastStartOutput);
				output += Environment.NewLine + qtFastStartOutput;
			}

			return result;
		}
	}
}