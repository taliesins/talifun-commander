using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command;
using Talifun.Commander.Command.PicasaUploader.Command.Request;
using Talifun.Commander.Command.PicasaUploader.CommandTester;
using Talifun.Commander.Command.PicasaUploader.Configuration;

namespace Talifun.Commander.Command.PicasaUploader
{
	public class PicasaUploaderService : CommandServiceBase<PicasaUploaderSaga, PicasaUploaderConfigurationTesterSaga>
	{
		public static IDictionary<IExecutePicasaUploaderWorkflowMessage, CancellableTask> Uploaders { get; set; }

		static PicasaUploaderService()
		{
			Settings = PicasaUploaderConfiguration.Instance;
			Uploaders = new ConcurrentDictionary<IExecutePicasaUploaderWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<RetrieveMetaDataMessageHandler>().Permanent();
				subscriber.Consumer<ExecutePicasaUploaderWorkflowMessageHandler>().Permanent();
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
