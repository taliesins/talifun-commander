using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command;
using Talifun.Commander.Command.FlickrUploader.Command.Request;
using Talifun.Commander.Command.FlickrUploader.CommandTester;
using Talifun.Commander.Command.FlickrUploader.Configuration;

namespace Talifun.Commander.Command.FlickrUploader
{
	public class FlickrUploaderService : CommandServiceBase<FlickrUploaderSaga, FlickrUploaderConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteFlickrUploaderWorkflowMessage, CancellableTask> Uploaders { get; set; }

		static FlickrUploaderService()
		{
			Settings = FlickrUploaderConfiguration.Instance;
			Uploaders = new ConcurrentDictionary<IExecuteFlickrUploaderWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<RetrieveMetaDataMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteFlickrUploaderWorkflowMessageHandler>().Permanent();
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
