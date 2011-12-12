using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.BoxNetUploader.Command;
using Talifun.Commander.Command.BoxNetUploader.Command.Request;
using Talifun.Commander.Command.BoxNetUploader.CommandTester;
using Talifun.Commander.Command.BoxNetUploader.Configuration;

namespace Talifun.Commander.Command.BoxNetUploader
{
	public class BoxNetUploaderService : CommandServiceBase<BoxNetUploaderSaga, BoxNetUploaderConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteBoxNetUploaderWorkflowMessage, CancellableTask> Uploaders { get; set; }

		static BoxNetUploaderService()
		{
			Settings = BoxNetUploaderConfiguration.Instance;
			Uploaders = new ConcurrentDictionary<IExecuteBoxNetUploaderWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<ExecuteBoxNetUploaderWorkflowMessageHandler>().Permanent();
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
