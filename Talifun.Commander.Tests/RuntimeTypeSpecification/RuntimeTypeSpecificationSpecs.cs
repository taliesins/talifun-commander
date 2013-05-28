using System;
using System.Reflection;
using MassTransit.RequestResponse.Configurators;
using NUnit.Framework;

namespace Talifun.Commander.Tests.RuntimeTypeSpecification
{
    [TestFixture]
    public class RuntimeTypeSpecificationSpecs
    {
        [Test]
        public void Dynamic()
        {
            var bus = Rhino.Mocks.MockRepository.GenerateStub<ITestServiceBus>();
            var message = new TestMessage
                {
                    Name = "Tali",
                    GetConfigureCallback = configurator => { }
                };

            var result = RuntimeTypeSpecificationSpecsExtensions.PublishRequest(bus, typeof(TestMessage), message, message.GetConfigureCallback);

            Assert.True(result);
        }
    }

    public interface ITestServiceBus
    {

    }

    public class TestMessage
    {
        public string Name { get; set; }
        public Action<InlineRequestConfigurator<dynamic>> GetConfigureCallback { get; set; }
    }

    public static class RuntimeTypeSpecificationSpecsExtensions
    {
        public static bool PublishRequest2<TRequest>(this ITestServiceBus bus, TRequest message, Action<InlineRequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            return true;
        }

        private static readonly MethodInfo GenericPublishRequestMethod = typeof(RuntimeTypeSpecificationSpecsExtensions).GetMethod("PublishRequest2");

        public static bool PublishRequest(this ITestServiceBus bus, Type type, dynamic message, Action<InlineRequestConfigurator<dynamic>> configureCallback)
        {
            var publishRequestMethod = GenericPublishRequestMethod.MakeGenericMethod(type);

            var m = Convert.ChangeType(message, type); //TestMessage

            var t = configureCallback.Method.GetGenericArguments();
            

            var cbMethod = configureCallback.Method; //InlineRequestConfigurator<Dynamic>

            var cb = Convert.ChangeType(cbMethod, typeof(InlineRequestConfigurator<TestMessage>));
            
            //configureCallback(type);


            //public static bool PublishRequest<TRequest>(this IServiceBus bus, TRequest message, Action<InlineRequestConfigurator<TRequest>> configureCallback) where TRequest : class

            return (bool)publishRequestMethod.Invoke(null, new object[] { bus, message, cb });
        }
    }
}
