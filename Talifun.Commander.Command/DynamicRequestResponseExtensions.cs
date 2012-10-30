using System;
using System.Reflection;
using MassTransit;
using MassTransit.RequestResponse.Configurators;

namespace Talifun.Commander.Command
{
    public static class DynamicRequestResponseExtensions
    {
        private static readonly MethodInfo GenericPublishRequestMethod = typeof(RequestResponseExtensions).GetMethod("PublishRequest");
        public static bool PublishRequest(this IServiceBus bus, Type type, dynamic message, Action<InlineRequestConfigurator<dynamic>> configureCallback)
        {
            var publishRequestMethod = GenericPublishRequestMethod.MakeGenericMethod(type);
            return (bool)publishRequestMethod.Invoke(null, new object[] { bus, message, configureCallback });
        }

        private static readonly MethodInfo GenericBeginPublishRequestMethod = typeof(RequestResponseExtensions).GetMethod("BeginPublishRequest");
        public static IAsyncResult BeginPublishRequest(this IServiceBus bus,
                                                                 Type type,
                                                                 dynamic message,
                                                                 AsyncCallback callback,
                                                                 object state,
                                                                 Action<InlineRequestConfigurator<dynamic>> configureCallback)
        {
            var publishRequestMethod = GenericBeginPublishRequestMethod.MakeGenericMethod(type);
            return (IAsyncResult)publishRequestMethod.Invoke(null, new object[] { bus, message, callback, state, configureCallback });
        }

        private static readonly MethodInfo GenericSendRequestMethod = typeof(RequestResponseExtensions).GetMethod("SendRequest");
        public static bool SendRequest(this IEndpoint endpoint,
                                            Type type,
                                            dynamic message,
                                            IServiceBus bus,
                                            Action<RequestConfigurator<dynamic>> configureCallback)
        {
            var publishRequestMethod = GenericSendRequestMethod.MakeGenericMethod(type);
            return (bool)publishRequestMethod.Invoke(null, new object[] { endpoint, message, bus, configureCallback });
        }

        private static readonly MethodInfo GenericBeginSendRequestMethod = typeof(RequestResponseExtensions).GetMethod("BeginSendRequest");
        public static IAsyncResult BeginSendRequest(this IEndpoint endpoint,
                                                        Type type,
                                                        dynamic message,
                                                        IServiceBus bus,
                                                        AsyncCallback callback,
                                                        object state,
                                                        Action<InlineRequestConfigurator<dynamic>> configureCallback)
        {
            var publishRequestMethod = GenericBeginSendRequestMethod.MakeGenericMethod(type);
            return (IAsyncResult)publishRequestMethod.Invoke(null, new object[] { endpoint, message, bus, callback, state, configureCallback });
        }
    }
}
