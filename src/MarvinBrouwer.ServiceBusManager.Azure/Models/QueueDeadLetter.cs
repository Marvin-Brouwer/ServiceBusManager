using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="IQueue"/>'s dead-letter Queue
/// </summary>
public sealed class QueueDeadLetter : AzureResource<IQueue>
{
	/// <inheritdoc cref="QueueDeadLetter"/>
	public QueueDeadLetter(IServiceBusNamespace serviceBus, Queue queue)
	{
		ServiceBus = serviceBus;
		InnerResource = queue.InnerResource;

		Queue = queue;
	}

	/// <inheritdoc />
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(InnerResource.Name);

	/// <summary>
	/// Reference to this <see cref="QueueDeadLetter"/>s parent <see cref="Models.Queue"/>
	/// </summary>
	public Queue Queue { get; }
}