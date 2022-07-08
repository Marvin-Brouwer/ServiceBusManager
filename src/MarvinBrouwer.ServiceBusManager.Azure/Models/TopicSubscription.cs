namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record TopicSubscription : ServiceBusResource
{
	public TopicSubscription(Topic topic, string name) : base(topic.ServiceBus, name)
	{
		Topic = topic;
		DeadLetter = new TopicSubscriptionDeadLetter(this, topic);
	}

	public Topic Topic { get; }
	public TopicSubscriptionDeadLetter DeadLetter { get; }

	public override ServiceBusResource GetServiceBusTarget() => Topic;
}