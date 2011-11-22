using System.IO;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Talifun.Commander.Command.Esb;
using TechTalk.SpecFlow;

namespace Talifun.Commander.Tests.MessageSerialization
{
	[Binding]
	public class MessageSerialization
	{
		private ICommandIdentifier _message = null;
		private string _serializedMessage = string.Empty;
		private ICommandIdentifier _deserializedMessage = null;

		[Given(@"a ""(.*)"" message")]
		public void GivenAMessage(string messageType)
		{
			_message = MessageRegistry.GetMessage(messageType);
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
					_deserializedMessage = (ICommandIdentifier)serializer.Deserialize(jsonReader, _message.GetType());
				}
			}
		}

		[Then(@"the result should be a matching message")]
		public void ThenTheResultShouldBeAMatchingMessage()
		{
			_deserializedMessage.ShouldHave().AllRuntimeProperties().EqualTo(_message);
		}
	}
}
