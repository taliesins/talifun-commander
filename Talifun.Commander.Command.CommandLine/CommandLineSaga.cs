using System;
using System.IO;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.FileMatcher;

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
			var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, commandLineSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                

                var commandLineParameters = GetCommandLineParameters(commandLineSetting);
                var commandLineCommand = new CommandLineCommand();

				var commandSucessful = commandLineCommand.Run(commandLineParameters, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

                if (commandSucessful)
                {
                    inputFilePath.MoveCompletedFileToOutputFolder(commandLineSetting.FileNameFormat, commandLineSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, inputFilePath, output, commandLineSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
				workingDirectoryPath.Cleanup();
            }
        }
    }
}