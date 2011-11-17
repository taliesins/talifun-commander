using System;
using System.Configuration;
using System.IO;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Talifun.Commander.Command;
using Talifun.Commander.Command.Configuration;
using TechTalk.SpecFlow;

namespace Talifun.Commander.Tests.MessageSerialization
{
	[Binding]
	public class MessageSerialization
	{
		private object _message = null;
		private Type _messageType = null;
		private string _serializedMessage = string.Empty;
		private object _deserializedMessage = null;

		[Given(@"a Cancel Command Message")]
		public void GivenACancelCommandMessage()
		{
			_message = new CommandCancelMessageTestDouble
			{
				CorrelationId = Guid.NewGuid()
			};
			_messageType = typeof(CommandCancelMessageTestDouble);
		}

		[Given(@"a Request Command Configuration Test Message")]
		public void GivenARequestCommandConfigurationTestMessage()
		{
			var project = new ProjectElement();
			var appSettings = new AppSettingsSection();
			appSettings.Settings.Add("Key", "Value");
			_message = new CommandConfigurationTestRequestMessageTestDouble
			{
				CorrelationId = Guid.NewGuid(),
				Project = project,
				AppSettings = appSettings.Settings.ToDictionary()
			};
			_messageType = typeof(CommandConfigurationTestRequestMessageTestDouble);
		}

		[Given(@"a Request Command Message")]
		public void GivenARequestCommandMessage()
		{
			_message = new CommandRequestMessageTestDouble
			{
				CorrelationId = Guid.NewGuid()
			};
			_messageType = typeof (CommandRequestMessageTestDouble);
		}

		[When(@"the message is serialized")]
		public void WhenTheMessageIsSerialized()
		{
			var serializer = JsonSerializer.Create(new JsonSerializerSettings());

			var stringBuilder = new StringBuilder();
			using (var stringWriter = new StringWriter(stringBuilder))
			{
				using (var jsonWriter = new JsonTextWriter(stringWriter))
				{
					serializer.Serialize(jsonWriter, _message);
					jsonWriter.Flush();
					stringWriter.Flush();
					_serializedMessage = stringBuilder.ToString();
				}
			}
		}

		[When(@"the message is deserialized")]
		public void WhenTheMessageIsDeserialized()
		{
			var serializer = JsonSerializer.Create(new JsonSerializerSettings());

			using (var stringReader = new StringReader(_serializedMessage))
			{
				using (var jsonReader = new JsonTextReader(stringReader))
				{
					_deserializedMessage = serializer.Deserialize(jsonReader, _messageType);
				}
			}
		}

		[Then(@"the result should be a matching message")]
		public void ThenTheResultShouldBeAMatchingMessage()
		{
			var serializer = JsonSerializer.Create(new JsonSerializerSettings());

			var stringBuilder = new StringBuilder();
			using (var stringWriter = new StringWriter(stringBuilder))
			{
				using (var jsonWriter = new JsonTextWriter(stringWriter))
				{
					serializer.Serialize(jsonWriter, _deserializedMessage);
					jsonWriter.Flush();
					stringWriter.Flush();
					Assert.AreEqual(_serializedMessage, stringBuilder.ToString());
				}
			}
		}
	}
}
