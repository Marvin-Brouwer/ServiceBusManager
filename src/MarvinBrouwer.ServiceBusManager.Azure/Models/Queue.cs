namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record Queue : ServiceBusResource
{
	public Queue(ServiceBus serviceBus, string name) : base(serviceBus, name)
	{
		DeadLetter = new QueueDeadLetter(this);
	}

	public QueueDeadLetter DeadLetter { get; }
	public override ServiceBusResource GetServiceBusTarget() => this;
}