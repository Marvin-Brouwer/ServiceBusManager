using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IQueue"/>'s dead-letter Queue
/// </summary>
public sealed class QueueDeadLetter : AzureResource
{
	/// <inheritdoc cref="QueueDeadLetter"/>
	public QueueDeadLetter(IAzureSubscription subscription, IServiceBusNamespace serviceBus, Queue queue)
	{
		AzureSubscription = subscription;
		ServiceBusId = serviceBus.Id;
		ServiceBusName = serviceBus.Name;

		Key = queue.Key;
		Id = queue.Id;
		Name = queue.Name;
	}

	/// <inheritdoc />
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(Name);
}