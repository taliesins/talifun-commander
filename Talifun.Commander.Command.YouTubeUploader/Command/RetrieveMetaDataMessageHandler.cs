using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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
			var allowedCategoriesFile = new FileInfo("categories.cat");
			var allowedCategories = GetAllowedCategories(allowedCategoriesFile);

			var metaDataFile = new FileInfo(message.InputFilePath + ".YouTubeUploader.json");
			var metaData = metaDataFile.Exists ? GetMetaData(metaDataFile) : new YouTubeMetaData(){Categories = new List<string>()};

			var unknownCategories = metaData.Categories.Where(x => !allowedCategories.Contains(x));

			if (unknownCategories.Any())
			{
				throw new Exception("Unknown media cateogry");
			}

			var retrievedMetaDataMessage = new RetrievedMetaDataMessage()
			{
				CorrelationId = message.CorrelationId,
				MetaData = metaData
			};

			var bus = BusDriver.Instance.GetBus(YouTubeUploaderService.BusName);
			bus.Publish(retrievedMetaDataMessage);
		}

		private List<string> GetAllowedCategories(FileInfo allowedCategoriesFile)
		{
			var xml = XDocument.Load(allowedCategoriesFile.FullName);
			XNamespace atomNameSpace = "http://www.w3.org/2005/Atom";

			var allowedCategories = xml.Descendants(atomNameSpace + "category")
				.Select(item => item.Attribute("term").Value).ToList();

			return allowedCategories;
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
