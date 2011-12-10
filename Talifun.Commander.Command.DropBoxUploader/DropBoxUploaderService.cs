using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.DropBoxUploader.Command;
using Talifun.Commander.Command.DropBoxUploader.Command.Request;
using Talifun.Commander.Command.DropBoxUploader.CommandTester;
using Talifun.Commander.Command.DropBoxUploader.Configuration;

namespace Talifun.Commander.Command.DropBoxUploader
{
	public class DropBoxUploaderService : CommandServiceBase<DropBoxUploaderSaga, DropBoxUploaderConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteDropBoxUploaderWorkflowMessage, CancellableTask> Uploaders { get; set; }

		static DropBoxUploaderService()
		{
			Settings = DropBoxUploaderConfiguration.Instance;
			Uploaders = new ConcurrentDictionary<IExecuteDropBoxUploaderWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<ExecuteDropBoxUploaderWorkflowMessageHandler>().Permanent();
			});
		}

		public override void OnStop()
		{
			foreach (var commandLineExecutor in Uploaders)
			{
				commandLineExecutor.Value.CancellationTokenSource.Cancel();
			}
		}
	}
}
