﻿using System.IO;
using Talifun.Commander.Command.AntiVirus.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
    public class AntiVirusSaga : CommandSagaBase
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
            var workingDirectoryPath = GetWorkingDirectoryPath(properties, antiVirusSetting.WorkingPath, uniqueProcessingNumber);

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
                        fileVirusFree = mcAfeeCommand.Run(mcAfeeSettings, properties.InputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                }

                if (!fileVirusFree)
                {
                    MoveCompletedFileToOutputFolder(workingFilePath, antiVirusSetting.FileNameFormat, antiVirusSetting.OutPutPath);
                }
                else
                {
                    HandleError(output, properties, antiVirusSetting.ErrorProcessingPath, uniqueProcessingNumber);
                }
            }
            finally
            {
                Cleanup(workingDirectoryPath);
            }
        }
    }
}