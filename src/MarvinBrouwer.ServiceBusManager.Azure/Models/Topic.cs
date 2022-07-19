using Microsoft.Azure.Management.ServiceBus.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="ITopic"/>
/// </summary>
public sealed class Topic : AzureResource<ITopic>
{
	/// <inheritdoc cref="Topic"/>
	public Topic(IServiceBusNamespace serviceBus, ITopic topic)
	{
		ServiceBus = serviceBus;
		InnerResource = topic;
	}

	/// <summary>
	/// This <see cref="Topic"/>'s subscribers
	/// </summary>
	public IAsyncEnumerable<TopicSubscription> TopicSubscriptions { get; init; } = default!;

}