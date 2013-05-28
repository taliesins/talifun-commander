using System;
using Magnum.Reflection;
using MassTransit;
using Newtonsoft.Json;

namespace Talifun.Commander.Command.Esb.Serialization
{
    public class InterfaceProxyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var target = FastActivator.Create(InterfaceImplementationBuilder.GetProxyFor(objectType));
            serializer.Populate(reader, target);
            return target;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsInterface && objectType.IsAllowedMessageType();
        }
    }
}
