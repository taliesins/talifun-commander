using System;
using System.IO;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.AntiVirus.Properties;
using Talifun.Commander.Command.FileMatcher;

namespace Talifun.Commander.Command.AntiVirus.Command
{
    public class OldAntiVirusSaga : CommandSagaBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return AntiVirusConfiguration.Instance;
            }
        }

		private IAntiVirusSettings GetCommandSettings(AntiVirusElement antiVirusSetting)
		{
			switch (antiVirusSetting.VirusScannerType)
			{
				case VirusScannerType.NotSpecified:
				case VirusScannerType.McAfee:
					return new McAfeeSettings();
				default:
					throw new Exception(Resource.ErrorMessageUnknownVirusScannerType);
			}
		}

		private ICommand<IAntiVirusSettings> GetCommand(IAntiVirusSettings antiVirusSetting)
		{
			return new McAfeeCommand();
		}

    	public override void Run(ICommandSagaProperties properties)
    	{
    		var commandElement = properties.Project.GetElement<AntiVirusElement>(properties.FileMatch, Settings.ElementCollectionSettingName);
    		var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, commandElement.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;

            	var commandSettings = GetCommandSettings(commandElement);
				var command = GetCommand(commandSettings);

				var fileVirusFree = command.Run(commandSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);

                if (!fileVirusFree)
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