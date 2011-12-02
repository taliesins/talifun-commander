using System.Collections.Generic;
using System.IO;
using MassTransit;
using Newtonsoft.Json;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.YouTubeUploader.Command.Request;
using Talifun.Commander.Command.YouTubeUploader.Command.Response;
using Talifun.Commander.Command.YouTubeUploader.Command.Settings;

namespace Talifun.Commander.Command.YouTubeUploader.Command
{
	public class RetrieveMetaDataMessageHandler : Consumes<RetrieveMetaDataMessage>.All
	{
		public void Consume(RetrieveMetaDataMessage message)
		{
			var metaDataFile = new FileInfo(message.InputFilePath + ".YouTubeUploader.json");
			var metaData = metaDataFile.Exists ? GetMetaData(metaDataFile) : new YouTubeMetaData(){Categories = new List<string>()};

			var retrievedMetaDataMessage = new RetrievedMetaDataMessage()
			{
				CorrelationId = message.CorrelationId,
				MetaData = metaData
			};

			var bus = BusDriver.Instance.GetBus(YouTubeUploaderService.BusName);
			bus.Publish(retrievedMetaDataMessage);
		}

		private YouTubeMetaData GetMetaData(FileInfo metaDataFile)
		{
			using (var textReader = metaDataFile.OpenText())
			{
				var json = textReader.ReadToEnd().Trim();
				var youTubeMetaData = JsonConvert.DeserializeObject<YouTubeMetaData>(json);
				return youTubeMetaData;
			}
		}
	}
}
