using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using MassTransit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Talifun.Commander.Command.Esb.Serialization
{
    public class JsonMessageSerializer : MassTransit.Serialization.IMessageSerializer
    {
        public string ContentType
        {
            get
            {
                return ContentTypeHeaderValue;
            }
        }

        public void Serialize<T>(Stream output, ISendContext<T> context)
            where T : class
        {
            try
            {
                context.SetContentType(ContentTypeHeaderValue);

                var envelope = MassTransit.Serialization.Envelope.Create(context);

                using (var nonClosingStream = new MassTransit.Serialization.Custom.NonClosingStream(output))
                using (var writer = new StreamWriter(nonClosingStream))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.Formatting = Formatting.Indented;

                    Serializer.Serialize(jsonWriter, envelope);

                    jsonWriter.Flush();
                    writer.Flush();
                }
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }

        public void Deserialize(IReceiveContext context)
        {
            try
            {
                MassTransit.Serialization.Envelope result;
                using (var nonClosingStream = new MassTransit.Serialization.Custom.NonClosingStream(context.BodyStream))
                using (var reader = new StreamReader(nonClosingStream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    result = Deserializer.Deserialize<MassTransit.Serialization.Envelope>(jsonReader);
                }

                MassTransit.Serialization.EnvelopeExtensions.SetUsingEnvelope(context, result);

                context.SetMessageTypeConverter(new JsonMessageTypeConverter(Deserializer, result.Message as JToken,
                    result.MessageType));
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize message", ex);
            }
        }

        const string ContentTypeHeaderValue = "application/vnd.talifun.commander+json";

        [ThreadStatic]
        private static JsonSerializer _deserializer;

        [ThreadStatic]
        private static JsonSerializer _serializer;

        public static JsonSerializer Serializer
        {
            get
            {
                var jsonSerializer = _serializer;
                if (jsonSerializer != null)
                    return jsonSerializer;
                var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        ContractResolver = new JsonContractResolver()
                    };
                var serializerSettings = settings;
                var jsonConverterArray = new JsonConverter[]
                    {
                      new ByteArrayConverter(),
                      null
                    };
                jsonConverterArray[1] = new IsoDateTimeConverter
                    {
                        DateTimeStyles = DateTimeStyles.RoundtripKind
                    };
                var list = new List<JsonConverter>(jsonConverterArray);
                serializerSettings.Converters = list;
                return _serializer = JsonSerializer.Create(settings);
            }
        }

        public static JsonSerializer Deserializer
        {
            get
            {
                var jsonSerializer = _deserializer;
                if (jsonSerializer != null)
                    return jsonSerializer;
                var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        ContractResolver = new JsonContractResolver()
                    };
                var serializerSettings = settings;
                var jsonConverterArray = new JsonConverter[]
                    {
                      new ByteArrayConverter(),
                      new ListJsonConverter(),
                      new InterfaceProxyConverter(),
                      new StringDecimalConverter(),
                      null
                    };
                jsonConverterArray[4] = new IsoDateTimeConverter
                    {
                        DateTimeStyles = DateTimeStyles.RoundtripKind
                    };
                var list = new List<JsonConverter>(jsonConverterArray);
                serializerSettings.Converters = list;
                return _deserializer = JsonSerializer.Create(settings);
            }
        }
    }
}
