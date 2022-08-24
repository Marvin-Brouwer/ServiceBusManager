using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.ServiceBus.Fluent;

using System.Runtime.CompilerServices;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureServiceBusService : IAzureServiceBusService
{
	private readonly IAzureAuthenticationService _authenticationService;

	/// <inheritdoc cref="AzureServiceBusService" />
	public AzureServiceBusService(IAzureAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<ServiceBus> ListServiceBuses(IAzureSubscription subscription, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		
		var credentials = await _authenticationService.Authenticate(subscription, cancellationToken);
		var azure = credentials.WithSubscription(subscription.SubscriptionId);

		var resourceGroups = await azure.ResourceGroups
			.ListAsync(true, cancellationToken);
		
		foreach (var resourceGroup in resourceGroups.OrderBy(r => r.Name))
		{
			var serviceBusNameSpaces = await azure.ServiceBusNamespaces
				.ListByResourceGroupAsync(resourceGroup.Name, true, cancellationToken);

			foreach (var serviceBusNamespace in serviceBusNameSpaces.OrderBy(s => s.Name))
			{
				yield return new ServiceBus(subscription, serviceBusNamespace);
			}
		}

	}

	/// <inheritdoc />
	public (IAsyncEnumerable<Queue> queues, IAsyncEnumerable<Topic> topics) ListServiceBusResources(ServiceBus serviceBus, CancellationToken cancellationToken)
	{
		var queues = GetQueues(serviceBus.Subscription, serviceBus.InnerResource, cancellationToken);
		var topics = GetTopics(serviceBus.Subscription, serviceBus.InnerResource, cancellationToken);

		return (queues, topics);
	}

	/// <inheritdoc />
	public IAsyncEnumerable<TopicSubscription> ListTopicSubscriptions(Topic topic, CancellationToken cancellationToken)
	{
		return GetSubscriptions(topic.Subscription, topic.ServiceBus, topic.InnerResource, cancellationToken);
	}

	private static async IAsyncEnumerable<Queue> GetQueues(IAzureSubscription subscription, IServiceBusNamespace serviceBusNamespace, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var queues = await serviceBusNamespace.Queues
			.ListAsync(true, cancellationToken);

		foreach (var queue in queues.OrderBy(q => q.Name))
		{
			yield return new Queue(subscription, serviceBusNamespace, queue);
		}
	}

	private static async IAsyncEnumerable<Topic> GetTopics(IAzureSubscription subscription, IServiceBusNamespace serviceBusNamespace, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var topics = await serviceBusNamespace.Topics
			.ListAsync(true, cancellationToken);

		foreach (var topic in topics.OrderBy(t => t.Name))
		{
			yield return new Topic(subscription, serviceBusNamespace, topic)
			{
				TopicSubscriptions = GetSubscriptions(subscription, serviceBusNamespace, topic, cancellationToken)
			};
		}
	}

	private static async IAsyncEnumerable<TopicSubscription> GetSubscriptions(
		IAzureSubscription subscription, IServiceBusNamespace serviceBusNamespace, ITopic topic,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var subscriptions = await topic.Subscriptions
			.ListAsync(true, cancellationToken);

		foreach (var topicSubscription in subscriptions.OrderBy(s => s.Name))
		{
			yield return new TopicSubscription(subscription, serviceBusNamespace, topic, topicSubscription);
		}
	}
}