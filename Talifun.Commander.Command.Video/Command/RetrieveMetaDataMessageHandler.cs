using System.IO;
using MassTransit;
using Newtonsoft.Json;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.Containers;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Command.Video.Command.Response;

namespace Talifun.Commander.Command.Video.Command
{
	public class RetrieveMetaDataMessageHandler : Consumes<RetrieveMetaDataMessage>.All
	{
		public void Consume(RetrieveMetaDataMessage message)
		{
			var metaDataFile = new FileInfo(message.InputFilePath + ".VideoConversion.json");
			var metaData = metaDataFile.Exists ? GetMetaData(metaDataFile) : new ContainerMetaData();

			var retrievedMetaDataMessage = new RetrievedMetaDataMessage()
			{
				CorrelationId = message.CorrelationId,
				MetaData = metaData
			};

			var bus = BusDriver.Instance.GetBus(VideoConversionService.BusName);
			bus.Publish(retrievedMetaDataMessage);
		}

		private ContainerMetaData GetMetaData(FileInfo metaDataFile)
		{
			using (var textReader = metaDataFile.OpenText())
			{
				var json = textReader.ReadToEnd().Trim();
				var containerMetaData = JsonConvert.DeserializeObject<ContainerMetaData>(json);
				return containerMetaData;
			}
		}
	}
}
