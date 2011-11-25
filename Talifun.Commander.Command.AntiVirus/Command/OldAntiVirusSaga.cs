using System;
using System.IO;
using Talifun.Commander.Command.AntiVirus.Configuration;
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

        public McAfeeSettings GetMcAfeeSettings(AntiVirusElement antiVirus)
        {
            return new McAfeeSettings();
        }

        public override void Run(ICommandSagaProperties properties)
        {
            var antiVirusSetting = GetSettings<AntiVirusElementCollection, AntiVirusElement>(properties);
			var uniqueProcessingNumber = Guid.NewGuid().ToString();
			var inputFilePath = new FileInfo(properties.InputFilePath);
			var workingDirectoryPath = inputFilePath.GetWorkingDirectoryPath(Settings.ConversionType, antiVirusSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                

                var fileVirusFree = false;

                switch (antiVirusSetting.VirusScannerType)
                {
                    case VirusScannerType.NotSpecified:
                    case VirusScannerType.McAfee:
                        var mcAfeeSettings = GetMcAfeeSettings(antiVirusSetting);
                        var mcAfeeCommand = new McAfeeCommand();
						fileVirusFree = mcAfeeCommand.Run(mcAfeeSettings, properties.AppSettings, inputFilePath, workingDirectoryPath, out inputFilePath, out output);
                        break;
                }

                if (!fileVirusFree)
                {
					inputFilePath.MoveCompletedFileToOutputFolder(antiVirusSetting.FileNameFormat, antiVirusSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, inputFilePath, output, antiVirusSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
				workingDirectoryPath.Cleanup();
            }
        }
    }
}