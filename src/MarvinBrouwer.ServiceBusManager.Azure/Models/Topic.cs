using Microsoft.Azure.Management.ServiceBus.Fluent;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Models;

/// <summary>
/// Representation of a <see cref="ITopic"/>
/// </summary>
public sealed class Topic : AzureResource<ITopic>
{
	/// <inheritdoc cref="Topic"/>
	public Topic(IAzureSubscription subscription, IServiceBusNamespace serviceBus, ITopic topic)
	{
		Subscription = subscription;
		ServiceBus = serviceBus;
		InnerResource = topic;
	}

	/// <summary>
	/// This <see cref="Topic"/>'s subscribers
	/// </summary>
	public IAsyncEnumerable<TopicSubscription> TopicSubscriptions { get; init; } = default!;

}