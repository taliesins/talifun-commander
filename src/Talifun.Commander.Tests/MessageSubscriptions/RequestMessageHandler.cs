using MassTransit;
using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Tests.MessageSubscriptions
{
	public class RequestMessageHandler : Consumes<RequestMessage>.All
	{
		public void Consume(RequestMessage message)
		{
			var responderBus = BusDriver.Instance.GetBus(MessageSubscriptions.ResponderName);

			var responseMessage = new ResponseMessage
			{
				CorrelationId = message.CorrelationId,
				TheAnswer = "Answer:" + message.TheQuestion,
			};

			responderBus.Context().Respond(responseMessage);
		}
	}
}