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
                return CommandLineSettingConfiguration.Instance;
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

        public override void Run(ICommandSagaProperties properties)
        {
            var commandLineSetting = GetSettings<CommandLineSettingElementCollection, CommandLineSettingElement>(properties);
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