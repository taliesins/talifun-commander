using System;
using System.Configuration;
using System.IO;
using System.Threading;
using NLog;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Properties;

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
            throw new IOException(string.Format(Resource.ErrorMessageUnableToDeleteDirectory, directory));
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
        	var sagaException = new SagaException(output, properties, errorProcessingPath, uniqueProcessingNumber);

			var logger = LogManager.GetLogger(Settings.ElementType.FullName);
			logger.ErrorException(output, sagaException);
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
            where E : NamedConfigurationElement, new()
        {
            var commandSettings = new ProjectElementCommand<C>(Settings.ElementCollectionSettingName, properties.Project);
            var settings = commandSettings.Settings;

            var commandSettingsKey = properties.FileMatch.CommandSettingsKey;

            var setting = settings[commandSettingsKey];
            if (setting == null)
                throw new ConfigurationErrorsException(string.Format(Resource.ErrorMessageFileMatchNoMatchingConversionSettingsKey, commandSettingsKey, Settings.ElementCollectionSettingName));
            return setting;
        }
    }
}
