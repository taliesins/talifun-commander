using System;
using System.Collections;
using System.Collections.Generic;
using Magnum.Extensions;
using Magnum.Reflection;
using Newtonsoft.Json;

namespace Talifun.Commander.Command.Esb.Serialization
{
    public class ListJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("This converter should not be used for writing as it can create loops");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.IsArray)
                return this.FastInvoke<ListJsonConverter, object>(new[]
                    {
                        objectType.GetElementType()
                    }, "GetSingleItemList", (object)reader, (object)serializer, (object)true);

            return this.FastInvoke<ListJsonConverter, object>(new[]
                {
                    objectType.GetGenericArguments()[0]
                }, "GetSingleItemList", (object)reader, (object)serializer, (object)false);
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsGenericType && (objectType.GetGenericTypeDefinition() == typeof(IList<>) || objectType.GetGenericTypeDefinition() == typeof(List<>)))
                return true;
            return objectType.IsArray && objectType.Implements<IEnumerable>();
        }

        private object GetSingleItemList<T>(JsonReader reader, JsonSerializer serializer, bool isArray)
        {
            var list = new List<T>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                serializer.Populate(reader, list);
            }
            else
            {
                var obj = (T)serializer.Deserialize(reader, typeof(T));
                list.Add(obj);
            }
            return isArray ? (object) list.ToArray() : list;
        }
    }
}
