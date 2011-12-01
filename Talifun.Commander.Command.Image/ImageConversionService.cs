using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.CommandTester;
using Talifun.Commander.Command.Image.Configuration;

namespace Talifun.Commander.Command.Image
{
	public class ImageConversionService : CommandServiceBase<ImageConversionSaga, ImageConversionConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteImageConversionWorkflowMessage, CancellableTask> CommandLineExecutors { get; set; }

		static ImageConversionService()
		{
			Settings = ImageConversionConfiguration.Instance;
			CommandLineExecutors = new Dictionary<IExecuteImageConversionWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteImageConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoErrorDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>().Permanent();
			});
		}

		public override void OnStop()
		{
			foreach (var commandLineExecutor in CommandLineExecutors)
			{
				commandLineExecutor.Value.CancellationTokenSource.Cancel();
			}
		}
	}
}
