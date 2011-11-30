using MassTransit;
using Talifun.Commander.Command.Audio.Command;
using Talifun.Commander.Command.Audio.CommandTester;
using Talifun.Commander.Command.Audio.Configuration;

namespace Talifun.Commander.Command.Audio
{
	public class AudioConversionService : CommandServiceBase<AudioConversionSaga, AudioConversionConfigurationTesterSaga>
	{
		static AudioConversionService()
		{
			Settings = AudioConversionConfiguration.Instance;
		}

		public override void Configure(MassTransit.BusConfigurators.ServiceBusConfigurator serviceBusConfigurator)
		{
			serviceBusConfigurator.Subscribe((subscriber) =>
			{
				subscriber.Consumer<CreateTempDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<ExecuteAudioConversionWorkflowMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoErrorDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<MoveProcessedFileIntoOutputDirectoryMessageHandler>().Permanent();
				subscriber.Consumer<DeleteTempDirectoryMessageHandler>().Permanent();
			});
		}
	}
}
