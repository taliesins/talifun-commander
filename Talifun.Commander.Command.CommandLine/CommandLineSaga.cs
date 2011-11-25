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

        private ICommandLineParameters GetCommandSettings(CommandLineElement commandLine)
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

		private ICommand<ICommandLineParameters> GetCommand(ICommandLineParameters commandLineSettings)
		{
			return new CommandLineCommand();
		}

        public override void Run(ICommandSagaProperties properties)
        {
			var commandElement = properties.Project.GetElement<CommandLineElement>(properties.FileMatch, Settings.ElementCollectionSettingName);

			var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, commandElement.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;

				var commandSettings = GetCommandSettings(commandElement);
				var command = GetCommand(commandSettings);

				var commandSuccessful = command.Run(commandSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

                if (commandSuccessful)
                {
                    inputFilePath.MoveCompletedFileToOutputFolder(commandElement.FileNameFormat, commandElement.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, inputFilePath, output, commandElement.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
				workingDirectoryPath.Cleanup();
            }
        }
    }
}