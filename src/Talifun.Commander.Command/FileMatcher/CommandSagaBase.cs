using System.IO;
using NLog;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FileMatcher
{
    public abstract class CommandSagaBase : ICommandSaga
    {
        public abstract ISettingConfiguration Settings { get; }
        public abstract void Run(ICommandSagaProperties properties);

		public void HandleError(ICommandSagaProperties properties, string uniqueProcessingNumber, FileInfo workingFilePath, string output, string errorProcessingPath)
        {
			var sagaException = new SagaException(properties, uniqueProcessingNumber, workingFilePath, output, errorProcessingPath);
			var logger = LogManager.GetLogger(Settings.ElementType.FullName);
			logger.ErrorException(output, sagaException);

			if (!workingFilePath.Exists || string.IsNullOrEmpty(errorProcessingPath)) return;

			var filename = workingFilePath.Name;
			var outputFilePath = new FileInfo(Path.Combine(errorProcessingPath, filename));
			if (outputFilePath.Exists)
			{
				outputFilePath.Delete();
			}

			workingFilePath.MoveTo(outputFilePath.FullName);
        }
    }
}
