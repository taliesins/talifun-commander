using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine
{
    public class CommandLineRunner : ICommandRunner
    {
        public string ConversionType
        {
            get
            {
                return CommandLineSettingConfiguration.ConversionType;
            }
        }

        private CommandLineParameters GetCommandLineParameters(CommandLineSettingElement commandLineSetting)
        {
            var args = commandLineSetting.Args
                .Replace("{%Name%}", commandLineSetting.Name)
                .Replace("{%OutPutPath%}", commandLineSetting.OutPutPath)
                .Replace("{%WorkingPath%}", commandLineSetting.WorkingPath)
                .Replace("{%ErrorProcessingPath%}", commandLineSetting.ErrorProcessingPath)
                .Replace("{%FileNameFormat%}", commandLineSetting.FileNameFormat)
                .Replace("{%CommandPath%}", commandLineSetting.CommandPath);

            var commandLineSettings = new CommandLineParameters
            {
                CommandArguments = args,
                CommandPath = commandLineSetting.CommandPath
            };
            return commandLineSettings;
        }

        public void Run(ICommanderManager commanderManager, FileInfo inputFilePath, ProjectElement project, FileMatchElement fileMatch)
        {
            var commandSettings = new ProjectElementCommand<CommandLineSettingElementCollection>(CommandLineSettingConfiguration.CollectionSettingName, project);
            var commandLineSettings = commandSettings.Settings;

            var commandLineSettingsKey = fileMatch.CommandSettingsKey;

            var commandLineSetting = commandLineSettings[commandLineSettingsKey];

            if (commandLineSetting == null)
                throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                       commandLineSetting +
                                                       "' does not match any key found in commandLineSettings name attributes");


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

                var commandLineParameters = GetCommandLineParameters(commandLineSetting);
                var commandLineCommand = new CommandLineCommand();

                var commandSucessful = commandLineCommand.Run(commandLineParameters, inputFilePath, workingDirectoryPath, out workingFilePath, out output);

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