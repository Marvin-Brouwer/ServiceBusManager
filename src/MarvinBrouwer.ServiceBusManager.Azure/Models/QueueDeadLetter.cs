using MarvinBrouwer.ServiceBusManager.Azure.Helpers;
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record QueueDeadLetter(IServiceBusNamespace ServiceBus, Queue Queue) : AzureResource<IQueue>(ServiceBus, Queue.InnerResource)
{
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(InnerResource.Name);
}