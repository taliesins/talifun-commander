using System;
using System.Reflection;
using MassTransit;

namespace Talifun.Commander.Command
{
	public static class DynamicPublishExtensions
	{
		private static readonly MethodInfo GenericPublishMethod = typeof(ServiceBus).GetMethod("Publish");
		public static void Publish(this IServiceBus bus, Type type, dynamic message)
		{
			var requestMethod = GenericPublishMethod.MakeGenericMethod(type);
			Action<IPublishContext<dynamic>> contextCallback = x => { };
			requestMethod.Invoke(bus, new object[] { message, contextCallback});
		}

		public static void Publish(this IServiceBus bus, Type type, dynamic message, Action<IPublishContext<dynamic>> contextCallback)
		{
			var requestMethod = GenericPublishMethod.MakeGenericMethod(type);
			requestMethod.Invoke(bus, new object[] { message, contextCallback });
		}
	}
}
