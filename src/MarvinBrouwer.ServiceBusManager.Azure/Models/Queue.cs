using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IQueue"/>
/// </summary>
public sealed class Queue : AzureResource<IQueue>
{
	/// <inheritdoc cref="Queue"/>
	public Queue(IServiceBusNamespace serviceBus, IQueue queue)
	{
		ServiceBus = serviceBus;
		InnerResource = queue;

		DeadLetter = new QueueDeadLetter(serviceBus, this);
	}

	/// <summary>
	/// Instance of this <see cref="Queue"/>s dead-letter <see cref="Queue"/>
	/// </summary>
	public QueueDeadLetter DeadLetter { get; }
}