using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record Queue : AzureResource<IQueue>
{
	public Queue(IQueue queue) : base(queue)
	{
		DeadLetter = new QueueDeadLetter(this);
	}

	public QueueDeadLetter DeadLetter { get; }
}