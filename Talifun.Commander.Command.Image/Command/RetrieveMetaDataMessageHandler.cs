using System.IO;
using MassTransit;
using Newtonsoft.Json;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.ImageSettings;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.Command.Response;

namespace Talifun.Commander.Command.Image.Command
{
	public class RetrieveMetaDataMessageHandler : Consumes<RetrieveMetaDataMessage>.All
	{
		public void Consume(RetrieveMetaDataMessage message)
		{
			var metaDataFile = new FileInfo(message.InputFilePath + ".meta.json");
			var metaData = metaDataFile.Exists ? GetMetaData(metaDataFile) : new ImageMetaData();

			var retrievedMetaDataMessage = new RetrievedMetaDataMessage()
			{
				CorrelationId = message.CorrelationId,
				MetaData = metaData
			};

			var bus = BusDriver.Instance.GetBus(ImageConversionService.BusName);
			bus.Publish(retrievedMetaDataMessage);
		}

		private ImageMetaData GetMetaData(FileInfo metaDataFile)
		{
			using (var textReader = metaDataFile.OpenText())
			{
				var json = textReader.ReadToEnd().Trim();
				var imageMetaData = JsonConvert.DeserializeObject<ImageMetaData>(json);
				return imageMetaData;
			}
		}
	}
}
