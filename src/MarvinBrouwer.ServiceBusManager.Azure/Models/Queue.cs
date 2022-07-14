using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record Queue : AzureResource<IQueue>
{
	public Queue(IServiceBusNamespace serviceBus, IQueue queue) : base(serviceBus, queue)
	{
		DeadLetter = new QueueDeadLetter(serviceBus, this);
	}

	public QueueDeadLetter DeadLetter { get; }
}