using System;
using System.Collections.Generic;
using System.Linq;
using Magnum.Reflection;
using MassTransit;
using MassTransit.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Talifun.Commander.Command.Esb.Serialization
{
    public class JsonMessageTypeConverter : IMessageTypeConverter
    {
        private readonly IDictionary<Type, object> _mapped;
        private readonly JsonSerializer _serializer;
        private readonly IEnumerable<string> _supportedTypes;
        private readonly JToken _token;

        public JsonMessageTypeConverter(JsonSerializer serializer, JToken token, IEnumerable<string> supportedTypes)
        {
            _token = token;
            _supportedTypes = supportedTypes;
            _serializer = serializer;
            _mapped = new Dictionary<Type, object>();
        }

        public bool Contains(Type messageType)
        {
            object obj;
            if (_mapped.TryGetValue(messageType, out obj))
                return obj != null;
            return _supportedTypes.Any(new MessageUrn(messageType).ToString().Equals);
        }

        public bool TryConvert<T>(out T message) where T : class
        {
            object obj1;
            if (_mapped.TryGetValue(typeof(T), out obj1))
            {
                message = (T)obj1;
                return message != null;
            }
            if (_supportedTypes.Any(new MessageUrn(typeof(T)).ToString().Equals))
            {
                object obj;
                if (typeof(T).IsInterface && typeof(T).IsAllowedMessageType())
                {
                    obj = FastActivator.Create(InterfaceImplementationBuilder.GetProxyFor(typeof(T)));
                    UsingReader(jsonReader => _serializer.Populate(jsonReader, obj));
                }
                else
                {
                    obj = FastActivator<T>.Create();
                    UsingReader(jsonReader => _serializer.Populate(jsonReader, obj));
                }
                _mapped[typeof(T)] = obj;
                message = (T)obj;
                return true;
            }
            _mapped[typeof(T)] = null;
            message = default(T);
            return false;
        }

        private void UsingReader(Action<JsonReader> callback)
        {
            if (_token == null)
                return;
            using (var jtokenReader = new JTokenReader(_token))
                callback(jtokenReader);
        }
    }
}
