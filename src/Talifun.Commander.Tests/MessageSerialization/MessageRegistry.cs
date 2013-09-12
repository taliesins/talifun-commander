using System;
using System.Configuration;
using Magnum;
using Talifun.Commander.Command;
using Talifun.Commander.Command.AntiVirus;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Audio.Configuration;
using Talifun.Commander.Command.BoxNetUploader.Configuration;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.DropBoxUploader.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.FlickrUploader.Configuration;
using Talifun.Commander.Command.Image.Configuration;
using Talifun.Commander.Command.PicasaUploader.Configuration;
using Talifun.Commander.Command.Video.Configuration;
using Talifun.Commander.Command.VideoThumbnailer.Configuration;
using Talifun.Commander.Command.YouTubeUploader.Configuration;

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

        private static void AddAudioConversionSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(AudioConversionConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<AudioConversionElementCollection>(configurationProperty);

            var element = new AudioConversionElement
            {
                Name = "AudioConversionElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddBoxNetUploaderSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(BoxNetUploaderConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<BoxNetUploaderElementCollection>(configurationProperty);

            var element = new BoxNetUploaderElement
            {
                Name = "BoxNetUploaderElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddCommandLineSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(CommandLineConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<CommandLineElementCollection>(configurationProperty);

            var element = new CommandLineElement
            {
                Name = "CommandLineElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddDropBoxUploaderSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(DropBoxUploaderConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<DropBoxUploaderElementCollection>(configurationProperty);

            var element = new DropBoxUploaderElement
            {
                Name = "DropBoxUploaderElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddFlickrUploaderSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(FlickrUploaderConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<FlickrUploaderElementCollection>(configurationProperty);

            var element = new FlickrUploaderElement
            {
                Name = "FlickrUploaderElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddImageConversionSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(ImageConversionConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<ImageConversionElementCollection>(configurationProperty);

            var element = new ImageConversionElement
            {
                Name = "ImageConversionElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddPicasaUploaderSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(PicasaUploaderConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<PicasaUploaderElementCollection>(configurationProperty);

            var element = new PicasaUploaderElement
            {
                Name = "PicasaUploaderElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddVideoConversionSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(VideoConversionConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<VideoConversionElementCollection>(configurationProperty);

            var element = new VideoConversionElement
            {
                Name = "VideoConversionElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddVideoThumbnailerSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(VideoThumbnailerConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<VideoThumbnailerElementCollection>(configurationProperty);

            var element = new VideoThumbnailerElement
            {
                Name = "VideoThumbnailerElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

        private static void AddYouTubeUploaderSetting(ProjectElement project)
        {
            var configurationProperty = project.GetConfigurationProperty(YouTubeUploaderConfiguration.Instance.ElementCollectionSettingName);
            var commandElementCollection = project.GetElementCollection<YouTubeUploaderElementCollection>(configurationProperty);

            var element = new YouTubeUploaderElement
            {
                Name = "YouTubeUploaderElement",
                OutPutPath = @"c:\",
            };

            commandElementCollection.Add(element);
        }

		private static CommandConfigurationTestRequestMessageTestDouble GetRequestCommandConfigurationTestMessage()
		{
		    var fileMatches = new FileMatchElementCollection
		    {
                new FileMatchElement
                    {
                        Name = "AntiVirusFileMatchElement",
                        Expression = @".+?\.(pdf|doc|xls|ppt)$",
                        ConversionType = "AntiVirus",
                        CommandSettingsKey = "AntiVirusElement"
                    }
		    };

			var folders = new FolderElementCollection
			{
				new FolderElement
					{
						Name = "Folder1",
						FolderToWatch = @"c:\",
						WorkingPath = @"c:\",
						CompletedPath = @"c:\",
                        FileMatches = fileMatches
					}
			};

			var project = new ProjectElement
			{
				Name = "test",
				Folders = folders,				
			};

			AddAntiVirusSetting(project);
		    AddAudioConversionSetting(project);
		    AddBoxNetUploaderSetting(project);
		    AddCommandLineSetting(project);
		    AddDropBoxUploaderSetting(project);
		    AddFlickrUploaderSetting(project);
		    AddImageConversionSetting(project);
		    AddPicasaUploaderSetting(project);
		    AddVideoConversionSetting(project);
		    AddVideoThumbnailerSetting(project);
		    AddYouTubeUploaderSetting(project);

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
