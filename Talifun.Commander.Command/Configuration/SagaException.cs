using System;

namespace Talifun.Commander.Command.Configuration
{
	[Serializable]
	public class SagaException : Exception
	{
		public SagaException(string message, ICommandSagaProperties properties, string errorProcessingPath, string uniqueProcessingNumber) : base(message)
		{
			Properties = properties;
			ErrorProcessingPath = errorProcessingPath;
			UniqueProcessingNumber = uniqueProcessingNumber;
		}

		public ICommandSagaProperties Properties { get; private set; }
		public string ErrorProcessingPath { get; private set; }
		public string UniqueProcessingNumber { get; private set; }
	}
}
