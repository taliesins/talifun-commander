using System.IO;
using MassTransit;
using Newtonsoft.Json;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.PicasaUploader.Command.Request;
using Talifun.Commander.Command.PicasaUploader.Command.Response;
using Talifun.Commander.Command.PicasaUploader.Command.Settings;

namespace Talifun.Commander.Command.PicasaUploader.Command
{
	public class RetrieveMetaDataMessageHandler : Consumes<RetrieveMetaDataMessage>.All
	{
		public void Consume(RetrieveMetaDataMessage message)
		{
			var metaDataFile = new FileInfo(message.InputFilePath + ".PicasaUploader.json");
			var metaData = metaDataFile.Exists ? GetMetaData(metaDataFile) : new PicasaMetaData();

			var retrievedMetaDataMessage = new RetrievedMetaDataMessage()
			{
				CorrelationId = message.CorrelationId,
				MetaData = metaData
			};

			var bus = BusDriver.Instance.GetBus(PicasaUploaderService.BusName);
			bus.Publish(retrievedMetaDataMessage);
		}

		private PicasaMetaData GetMetaData(FileInfo metaDataFile)
		{
			using (var textReader = metaDataFile.OpenText())
			{
				var json = textReader.ReadToEnd().Trim();
				var youTubeMetaData = JsonConvert.DeserializeObject<PicasaMetaData>(json);
				return youTubeMetaData;
			}
		}
	}
}
