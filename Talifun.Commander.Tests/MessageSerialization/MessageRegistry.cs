using System;
using System.Configuration;
using Magnum;
using Talifun.Commander.Command;
using Talifun.Commander.Command.AntiVirus;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Tests.MessageSerialization
{
	public class MessageRegistry
	{
		public static ICommandIdentifier GetMessage(string messageType)
		{
			switch (messageType)
			{
				case "Cancel Command":
					return GetCancelCommandMessage();
				case "Request Command Configuration Test":
					return GetRequestCommandConfigurationTestMessage();
				case "Request Command":
					return GetRequestCommandMessage();
				default:
					throw new Exception(string.Format("Unknown message type - {0}", messageType) );
			}
		}

		private static CommandCancelMessageTestDouble GetCancelCommandMessage()
		{
			return new CommandCancelMessageTestDouble
			{
				CorrelationId = CombGuid.Generate()
			};
		}

		private static void AddAntiVirusSetting(ProjectElement project)
		{
			var configurationProperty = project.GetConfigurationProperty(AntiVirusConfiguration.Instance.ElementCollectionSettingName);
			var commandElementCollection = project.GetElementCollection<AntiVirusElementCollection>(configurationProperty);

			var element = new AntiVirusElement
			{
				Name = "AntiVirusElement",
				OutPutPath = @"c:\",
				VirusScannerType = VirusScannerType.NotSpecified,
			};

			commandElementCollection.Add(element);
		}

		private static CommandConfigurationTestRequestMessageTestDouble GetRequestCommandConfigurationTestMessage()
		{
			var folders = new FolderElementCollection()
			{
				new FolderElement()
					{
						Name = "Folder1",
						FolderToWatch = @"c:\",
						WorkingPath = @"c:\",
						CompletedPath = @"c:\"
					}
			};

			var project = new ProjectElement()
			{
				Name = "test",
				Folders = folders,				
			};

			AddAntiVirusSetting(project);

			var appSettings = new AppSettingsSection();
			appSettings.Settings.Add("Key", "Value");

			return new CommandConfigurationTestRequestMessageTestDouble
			{
				CorrelationId = CombGuid.Generate(),
				ProjectName = project.Name,
				AppSettings = appSettings.Settings.ToDictionary()
			};
		}

		private static PluginRequestMessageTestDouble GetRequestCommandMessage()
		{
			return new PluginRequestMessageTestDouble
			{
				CorrelationId = CombGuid.Generate()
			};
		}
	}
}
