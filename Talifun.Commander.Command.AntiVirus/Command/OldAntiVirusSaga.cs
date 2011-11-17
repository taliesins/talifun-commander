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
            var uniqueProcessingNumber = UniqueIdentifier();
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, antiVirusSetting.GetWorkingPathOrDefault(), uniqueProcessingNumber);

            try
            {
                workingDirectoryPath.Create();

                var output = string.Empty;
                FileInfo workingFilePath = null;

                var fileVirusFree = false;

                switch (antiVirusSetting.VirusScannerType)
                {
                    case VirusScannerType.NotSpecified:
                    case VirusScannerType.McAfee:
                        var mcAfeeSettings = GetMcAfeeSettings(antiVirusSetting);
                        var mcAfeeCommand = new McAfeeCommand();
                        fileVirusFree = mcAfeeCommand.Run(mcAfeeSettings, properties.AppSettings, new FileInfo(properties.InputFilePath), workingDirectoryPath, out workingFilePath, out output);
                        break;
                }

                if (!fileVirusFree)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, antiVirusSetting.FileNameFormat, antiVirusSetting.GetOutPutPathOrDefault());
                }
                else
                {
					HandleError(properties, uniqueProcessingNumber, workingFilePath, output, antiVirusSetting.GetErrorProcessingPathOrDefault());
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}