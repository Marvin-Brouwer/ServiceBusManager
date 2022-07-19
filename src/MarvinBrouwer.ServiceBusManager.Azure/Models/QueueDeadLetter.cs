using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed class QueueDeadLetter : AzureResource<IQueue>
{
	public QueueDeadLetter(IServiceBusNamespace serviceBus, Queue queue)
	{
		ServiceBus = serviceBus;
		InnerResource = queue.InnerResource;

		Queue = queue;
	}

	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(InnerResource.Name);
	public Queue Queue { get; }
}