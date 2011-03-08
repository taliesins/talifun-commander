using System;
using System.IO;
using Talifun.Commander.Command.CommandLine;
using Talifun.Commander.Configuration.Project.CommandLine;

namespace Talifun.Commander.MediaConversion
{
    public class CommandLineRunner : ICommandRunner<CommandLineSettingElement>
    {
        private CommandLineSettings GetCommandLineSettings(CommandLineSettingElement commandLineSetting)
        {
            var args = commandLineSetting.Args
                .Replace("{%Name%}", commandLineSetting.Name)
                .Replace("{%OutPutPath%}", commandLineSetting.OutPutPath)
                .Replace("{%WorkingPath%}", commandLineSetting.WorkingPath)
                .Replace("{%ErrorProcessingPath%}", commandLineSetting.ErrorProcessingPath)
                .Replace("{%FileNameFormat%}", commandLineSetting.FileNameFormat)
                .Replace("{%CommandPath%}", commandLineSetting.CommandPath);

            var commandLineSettings = new CommandLineSettings
            {
                CommandArguments = args,
                CommandPath = commandLineSetting.CommandPath
            };
            return commandLineSettings;
        }

        public void Run(ICommanderManager commanderManager, FileInfo inputFilePath, CommandLineSettingElement commandLineSetting)
        {
            var uniqueProcessingNumber = Guid.NewGuid().ToString();
            var uniqueDirectoryName = "commandLine." + inputFilePath.Name + "." + uniqueProcessingNumber;

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(commandLineSetting.WorkingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(commandLineSetting.WorkingPath, uniqueDirectoryName));
            }
            else
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));
            }

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var commandLineSettings = GetCommandLineSettings(commandLineSetting);
                var commandLineCommand = new CommandLineCommand();

                var commandSucessful = commandLineCommand.Run(commandLineSettings, inputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (commandSucessful)
                {
                    var filename = workingFilePath.Name;

                    if (!string.IsNullOrEmpty(commandLineSetting.FileNameFormat))
                    {
                        filename = string.Format(commandLineSetting.FileNameFormat, filename);
                    }

                    var outputFilePath = new FileInfo(Path.Combine(commandLineSetting.OutPutPath, filename));
                    if (outputFilePath.Exists)
                    {
                        outputFilePath.Delete();
                    }

                    workingFilePath.MoveTo(outputFilePath.FullName);
                }
                else
                {
                    FileInfo errorProcessingFilePath = null;
                    if (!string.IsNullOrEmpty(commandLineSetting.ErrorProcessingPath))
                    {
                        errorProcessingFilePath = new FileInfo(Path.Combine(commandLineSetting.ErrorProcessingPath, uniqueProcessingNumber + "." + inputFilePath.Name));
                    }

                    if (errorProcessingFilePath == null)
                    {
                        var exceptionOccurred = new Exception(output);
                        commanderManager.LogException(null, exceptionOccurred);
                    }
                    else
                    {
                        if (errorProcessingFilePath.Exists)
                        {
                            errorProcessingFilePath.Delete();
                        }

                        var errorProcessingLogFilePath = new FileInfo(errorProcessingFilePath.FullName + ".txt");

                        if (errorProcessingLogFilePath.Exists)
                        {
                            errorProcessingLogFilePath.Delete();
                        }

                        var exceptionOccurred = new Exception(output);
                        commanderManager.LogException(errorProcessingLogFilePath, exceptionOccurred);

                        inputFilePath.CopyTo(errorProcessingFilePath.FullName);
                    }
                }
            }
            finally
            {
                if (workingDirectoryPath.Exists)
                {
                    workingDirectoryPath.Delete(true);
                }
            }
        }
    }
}