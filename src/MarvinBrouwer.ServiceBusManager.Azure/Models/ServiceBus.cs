namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

public sealed record ServiceBus(
	string Name, string Secret)
{
	public IEnumerable<Topic> Topics { get; set; } = Enumerable.Empty<Topic>();
	public IEnumerable<Queue> Queues { get; set; } = Enumerable.Empty<Queue>();
}