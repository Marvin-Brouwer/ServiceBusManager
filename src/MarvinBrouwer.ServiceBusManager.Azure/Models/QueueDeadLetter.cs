using MarvinBrouwer.ServiceBusManager.Azure.Helpers;
using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record QueueDeadLetter(Queue Queue) : AzureResource<IQueue>(Queue.InnerResource)
{
}