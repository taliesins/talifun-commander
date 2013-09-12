using System.IO;
using MassTransit;
using Newtonsoft.Json;
using Talifun.Commander.Command.Audio.Command.AudioFormats;
using Talifun.Commander.Command.Audio.Command.Request;
using Talifun.Commander.Command.Audio.Command.Response;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Audio.Command
{
	public class RetrieveMetaDataMessageHandler : Consumes<RetrieveMetaDataMessage>.All
	{
		public void Consume(RetrieveMetaDataMessage message)
		{
			var metaDataFile = new FileInfo(message.InputFilePath + ".AudioConversion.json");
			var metaData = metaDataFile.Exists ? GetMetaData(metaDataFile) : new AudioMetaData();

			var retrievedMetaDataMessage = new RetrievedMetaDataMessage()
			{
				CorrelationId = message.CorrelationId,
				MetaData = metaData
			};

			var bus = BusDriver.Instance.GetBus(AudioConversionService.BusName);
			bus.Publish(retrievedMetaDataMessage);
		}

		private AudioMetaData GetMetaData(FileInfo metaDataFile)
		{
			using (var textReader = metaDataFile.OpenText())
			{
				var json = textReader.ReadToEnd().Trim();
				var audioMetaData = JsonConvert.DeserializeObject<AudioMetaData>(json);
				return audioMetaData;
			}
		}
	}
}
