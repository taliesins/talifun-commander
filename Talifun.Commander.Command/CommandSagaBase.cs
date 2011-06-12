using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    public abstract class CommandSagaBase : ICommandSaga
    {
        public abstract ISettingConfiguration Settings { get; }
        public abstract void Run(ICommandSagaProperties properties);

        public string UniqueIdentifier()
        {
            return Guid.NewGuid().ToString();
        }

        public void Cleanup(DirectoryInfo workingDirectoryPath)
        {
            if (workingDirectoryPath.Exists)
            {
                RetryDelete(workingDirectoryPath, 5, true);
            }
        }

        private void RetryDelete(DirectoryInfo directory, int retry, bool recursively)
        {
            var delay = 0;

            for (var i = 0; i < retry; i++)
            {
                try
                {
                    directory.Delete(recursively);
                    return;
                }
                catch (DirectoryNotFoundException)
                {
                    throw;
                }
                catch (IOException)
                {
                    delay += 100;
                    if (i == retry) throw;
                }

                Thread.Sleep(delay);
            }

            //We will never get here
            throw new IOException("Unable to delete directory " + directory);
        }

        public void MoveCompletedFileToOutputFolder(FileInfo workingFilePath, string fileNameFormat, string outPutPath)
        {
            var filename = workingFilePath.Name;

            if (!string.IsNullOrEmpty(fileNameFormat))
            {
                filename = string.Format(fileNameFormat, filename);
            }

            var outputFilePath = new FileInfo(Path.Combine(outPutPath, filename));
            if (outputFilePath.Exists)
            {
                outputFilePath.Delete();
            }

            workingFilePath.MoveTo(outputFilePath.FullName);
        }

        public void HandleError(string output, ICommandSagaProperties properties, string errorProcessingPath, string uniqueProcessingNumber)
        {
            FileInfo errorProcessingFilePath = null;
            if (!string.IsNullOrEmpty(errorProcessingPath))
            {
                errorProcessingFilePath = new FileInfo(Path.Combine(errorProcessingPath, uniqueProcessingNumber + "." + properties.InputFilePath.Name));
            }

            if (errorProcessingFilePath == null)
            {
                var exceptionOccurred = new Exception(output);
                properties.CommanderManager.LogException(null, exceptionOccurred);
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
                properties.CommanderManager.LogException(errorProcessingLogFilePath, exceptionOccurred);

                if (properties.InputFilePath.Exists)
                {
                    properties.InputFilePath.CopyTo(errorProcessingFilePath.FullName);
                }
            }
        }

        public DirectoryInfo GetWorkingDirectoryPath(ICommandSagaProperties properties, string workingPath, string uniqueProcessingNumber)
        {
            var uniqueDirectoryName = Settings.ConversionType + "." + properties.InputFilePath.Name + "." + uniqueProcessingNumber;

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(workingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(workingPath, uniqueDirectoryName));
            }
            else
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));
            }

            return workingDirectoryPath;
        }

        public E GetSettings<C, E>(ICommandSagaProperties properties)
            where C : CurrentConfigurationElementCollection<E> 
            where E : ConfigurationElement, new()
        {
            var commandSettings = new ProjectElementCommand<C>(Settings.CollectionSettingName, properties.Project);
            var settings = commandSettings.Settings;

            var commandSettingsKey = properties.FileMatch.CommandSettingsKey;

            var setting = settings[commandSettingsKey];
            if (setting == null)
                throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                       setting +
                                                       "' does not match any key found in " + Settings.CollectionSettingName + " name attributes");
            return setting;
        }
    }
}
