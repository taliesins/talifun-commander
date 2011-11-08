using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Esb
{
	[Serializable]
	public class FileMatchedSaga : SagaStateMachine<FileMatchedSaga>, ISaga
	{
		static FileMatchedSaga()
		{
		}

		// ReSharper disable UnusedMember.Global
		public static State Initial { get; set; }
		// ReSharper restore UnusedMember.Global
		// ReSharper disable UnusedMember.Global
		public static State Completed { get; set; }
		// ReSharper restore UnusedMember.Global

		public FileMatchedSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public Guid CorrelationId { get; private set; }
		public IServiceBus Bus { get; set; }


	}
}
