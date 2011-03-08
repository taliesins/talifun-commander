using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus
{
    public class AntiVirusRunner : ICommandRunner
    {
        public string ConversionType
        {
            get
            {
                return AntiVirusSettingConfiguration.ElementSettingName;
            }
        }

        public McAfeeSettings GetMcAfeeSettings(AntiVirusSettingElement antiVirusSetting)
        {
            return new McAfeeSettings();
        }

        public void Run(ICommanderManager commanderManager, FileInfo inputFilePath, ProjectElement project, FileMatchElement fileMatch)
        {
            var commandSettings = new ProjectElementCommand<AntiVirusSettingElementCollection>(AntiVirusSettingConfiguration.CollectionSettingName, project);
            var antiVirusSettings = commandSettings.Settings;

            var commandSettingsKey = fileMatch.CommandSettingsKey;

            var antiVirusSetting = antiVirusSettings[commandSettingsKey];
            if (antiVirusSetting == null)
                throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                       antiVirusSetting +
                                                       "' does not match any key found in antiVirusSettings name attributes");


            var uniqueProcessingNumber = Guid.NewGuid().ToString();
            var uniqueDirectoryName = "antiVirus." + inputFilePath.Name + "." + uniqueProcessingNumber;

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(antiVirusSetting.WorkingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(antiVirusSetting.WorkingPath, uniqueDirectoryName));
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

                var fileVirusFree = false;

                switch (antiVirusSetting.VirusScannerType)
                {
                    case VirusScannerType.NotSpecified:
                    case VirusScannerType.McAfee:
                        var mcAfeeSettings = GetMcAfeeSettings(antiVirusSetting);
                        var mcAfeeCommand = new McAfeeCommand();
                        fileVirusFree = mcAfeeCommand.Run(mcAfeeSettings, inputFilePath, workingDirectoryPath, out workingFilePath, out output);
                        break;
                }

                if (!fileVirusFree)
                {
                    var filename = workingFilePath.Name;

                    if (!string.IsNullOrEmpty(antiVirusSetting.FileNameFormat))
                    {
                        filename = string.Format(antiVirusSetting.FileNameFormat, filename);
                    }

                    var outputFilePath = new FileInfo(Path.Combine(antiVirusSetting.OutPutPath, filename));
                    if (outputFilePath.Exists)
                    {
                        outputFilePath.Delete();
                    }

                    workingFilePath.MoveTo(outputFilePath.FullName);
                }
                else
                {
                    FileInfo errorProcessingFilePath = null;
                    if (!string.IsNullOrEmpty(antiVirusSetting.ErrorProcessingPath))
                    {
                        errorProcessingFilePath = new FileInfo(Path.Combine(antiVirusSetting.ErrorProcessingPath, uniqueProcessingNumber + "." + inputFilePath.Name));
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

                        //Anti-virus program may delete file but we will have log file at least
                        if (inputFilePath.Exists)
                        {
                            inputFilePath.CopyTo(errorProcessingFilePath.FullName);
                        }
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