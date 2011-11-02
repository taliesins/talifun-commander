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
                .Replace("{%OutPutPath%}", commandLine.GetOutPutPathOrDefault())
                .Replace("{%WorkingPath%}", commandLine.GetWorkingPathOrDefault())
                .Replace("{%ErrorProcessingPath%}", commandLine.GetErrorProcessingPathOrDefault())
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
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, commandLineSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var commandLineParameters = GetCommandLineParameters(commandLineSetting);
                var commandLineCommand = new CommandLineCommand();

                var commandSucessful = commandLineCommand.Run(commandLineParameters, properties.AppSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);

                if (commandSucessful)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, commandLineSetting.FileNameFormat, commandLineSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, workingFilePath, output, commandLineSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}