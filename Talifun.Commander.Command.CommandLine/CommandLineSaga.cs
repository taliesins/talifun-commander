using System.IO;
using Talifun.Commander.Command.CommandLine.Configuration;

namespace Talifun.Commander.Command.CommandLine
{
    public class CommandLineSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return CommandLineConfiguration.Instance;
            }
        }

        private CommandLineParameters GetCommandLineParameters(CommandLineElement commandLine)
        {
            var args = commandLine.Args
                .Replace("{%Name%}", commandLine.Name)
                .Replace("{%OutPutPath%}", commandLine.OutPutPath)
                .Replace("{%WorkingPath%}", commandLine.WorkingPath)
                .Replace("{%ErrorProcessingPath%}", commandLine.ErrorProcessingPath)
                .Replace("{%FileNameFormat%}", commandLine.FileNameFormat)
                .Replace("{%CommandPath%}", commandLine.CommandPath);

            var commandLineSettings = new CommandLineParameters
            {
                CommandArguments = args,
                CommandPath = commandLine.CommandPath
            };
            return commandLineSettings;
        }

        public override void Run(ICommandSagaProperties properties)
        {
            var commandLineSetting = GetSettings<CommandLineElementCollection, CommandLineElement>(properties);
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, commandLineSetting.WorkingPath, uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var commandLineParameters = GetCommandLineParameters(commandLineSetting);
                var commandLineCommand = new CommandLineCommand();

                var commandSucessful = commandLineCommand.Run(commandLineParameters, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (commandSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, commandLineSetting.FileNameFormat, commandLineSetting.OutPutPath);
                }
                else
                {
                    HandleError(output, properties, commandLineSetting.ErrorProcessingPath, uniqueProcessingNumber);
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}