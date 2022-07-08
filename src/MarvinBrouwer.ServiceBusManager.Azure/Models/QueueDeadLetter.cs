using MarvinBrouwer.ServiceBusManager.Azure.Helpers;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record QueueDeadLetter(Queue Queue) : ServiceBusResource(
	Queue.ServiceBus,
	$"{Queue.Name}_{AzureConstants.DeadLetterPathSegment}")
{
	public override string Path => DeadLetterNameHelper.FormatDeadLetterPath(Queue.Path);
	public override ServiceBusResource GetServiceBusTarget() => Queue;
}