namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record Topic(ServiceBus ServiceBus, string Name) : ServiceBusResource(ServiceBus, Name)
{
	public IEnumerable<TopicSubscription> Subscriptions { get; set; } = Enumerable.Empty<TopicSubscription>();
    
	public override ServiceBusResource GetServiceBusTarget() => this;
}