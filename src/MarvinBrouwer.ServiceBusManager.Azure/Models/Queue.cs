using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed class Queue : AzureResource<IQueue>
{
	public Queue(IServiceBusNamespace serviceBus, IQueue queue)
	{
		ServiceBus = serviceBus;
		InnerResource = queue;

		DeadLetter = new QueueDeadLetter(serviceBus, this);
	}

	public QueueDeadLetter DeadLetter { get; }
}