using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IQueue"/>
/// </summary>
public sealed class Queue : AzureResource
{
	/// <inheritdoc cref="Queue"/>
	public Queue(IAzureSubscription subscription, IServiceBusNamespace serviceBus, IQueue queue)
	{
		AzureSubscription = subscription;
		ServiceBusId = serviceBus.Id;
		ServiceBusName = serviceBus.Name;

		Key = queue.Key;
		Id = queue.Id;
		Name = queue.Name;

		DeadLetter = new QueueDeadLetter(subscription, serviceBus, this);
	}

	/// <summary>
	/// Instance of this <see cref="Queue"/>s dead-letter <see cref="Queue"/>
	/// </summary>
	private QueueDeadLetter DeadLetter { get; }
}