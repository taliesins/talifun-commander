using System;
using System.IO;

namespace Talifun.Commander.Command.Configuration
{
	[Serializable]
	public class SagaException : Exception
	{
		public SagaException(ICommandSagaProperties properties, string uniqueProcessingNumber, FileInfo workingFilePath, string output, string errorProcessingPath)
			: base(output)
		{
			Properties = properties;
			UniqueProcessingNumber = uniqueProcessingNumber;
			WorkingFilePath = workingFilePath;
			ErrorProcessingPath = errorProcessingPath;
		}

		public ICommandSagaProperties Properties { get; private set; }
		public string UniqueProcessingNumber { get; private set; }
		public FileInfo WorkingFilePath { get; private set; }
		public string ErrorProcessingPath { get; private set; }
	}
}
