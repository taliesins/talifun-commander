using System.IO;
using MassTransit;
using Newtonsoft.Json;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Command.Request;
using Talifun.Commander.Command.FlickrUploader.Command.Response;
using Talifun.Commander.Command.FlickrUploader.Command.Settings;

namespace Talifun.Commander.Command.FlickrUploader.Command
{
	public class RetrieveMetaDataMessageHandler : Consumes<RetrieveMetaDataMessage>.All
	{
		public void Consume(RetrieveMetaDataMessage message)
		{
			var metaDataFile = new FileInfo(message.InputFilePath + ".FlickrUploader.json");
			var metaData = metaDataFile.Exists ? GetMetaData(metaDataFile) : new FlickrMetaData();

			var retrievedMetaDataMessage = new RetrievedMetaDataMessage()
			{
				CorrelationId = message.CorrelationId,
				MetaData = metaData
			};

			var bus = BusDriver.Instance.GetBus(FlickrUploaderService.BusName);
			bus.Publish(retrievedMetaDataMessage);
		}

		private FlickrMetaData GetMetaData(FileInfo metaDataFile)
		{
			using (var textReader = metaDataFile.OpenText())
			{
				var json = textReader.ReadToEnd().Trim();
				var flickrMetaData = JsonConvert.DeserializeObject<FlickrMetaData>(json);
				return flickrMetaData;
			}
		}
	}
}
