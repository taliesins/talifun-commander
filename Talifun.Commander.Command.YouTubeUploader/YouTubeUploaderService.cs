using System.Collections.Concurrent;
using System.Collections.Generic;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.YouTubeUploader.Command;
using Talifun.Commander.Command.YouTubeUploader.Command.Request;
using Talifun.Commander.Command.YouTubeUploader.CommandTester;
using Talifun.Commander.Command.YouTubeUploader.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader
{
	public class YouTubeUploaderService : CommandServiceBase<YouTubeUploaderSaga, YouTubeUploaderConfigurationTesterSaga>
	{
		public static IDictionary<IExecuteYouTubeUploaderWorkflowMessage, CancellableTask> Uploaders { get; set; }

		static YouTubeUploaderService()
		{
			Settings = YouTubeUploaderConfiguration.Instance;
			Uploaders = new ConcurrentDictionary<IExecuteYouTubeUploaderWorkflowMessage, CancellableTask>();
		}

		public override void OnConfigure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<RetrieveMetaDataMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteYouTubeUploaderWorkflowMessageHandler>().Permanent();
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
