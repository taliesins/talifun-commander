using System.Configuration;
using System.IO;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.CommandLine
{
    public class CommandLineCommand : ICommand<ICommandLineParameters>
    {
		public bool Run(ICommandLineParameters settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, inputFilePath.Name));
            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

            var workingDirectory = outputDirectoryPath.FullName;
            settings.CommandArguments = settings.CommandArguments.Replace("{%InputFilePath%}", inputFilePath.FullName).Replace("{%OutPutFilePath%}", outPutFilePath.FullName);

            var commandLineExecutor = new CommandLineExecutor();
            return commandLineExecutor.Execute(workingDirectory, settings.CommandPath, settings.CommandArguments, out output);
        }
    }
}