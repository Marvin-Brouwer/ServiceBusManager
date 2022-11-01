using MarvinBrouwer.ServiceBusManager.Azure.Models;

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;

using System.Runtime.CompilerServices;

using IAzureSubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

/// <inheritdoc />
public sealed class AzureServiceBusService : IAzureServiceBusService
{
	/// <inheritdoc />
	public async IAsyncEnumerable<ServiceBus> ListServiceBuses(IAzure azure, IAzureSubscription subscription,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
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
	public async Task<(IAsyncEnumerable<Queue> queues, IAsyncEnumerable<Topic> topics)> ListServiceBusResources(
		IAzure azure, ServiceBus serviceBus, CancellationToken cancellationToken)
	{
		var serviceBusNamespace = await azure.ServiceBusNamespaces
			.GetByIdAsync(serviceBus.Id, cancellationToken);

		var queues = GetQueues(serviceBus.AzureSubscription, serviceBusNamespace, cancellationToken);
		var topics = GetTopics(serviceBus.AzureSubscription, serviceBusNamespace, cancellationToken);

		return (queues, topics);
	}

	/// <inheritdoc />
	public async Task<IAsyncEnumerable<TopicSubscription>> ListTopicSubscriptions(IAzure azure, Topic topic, CancellationToken cancellationToken)
	{
		var serviceBusNamespace = await azure.ServiceBusNamespaces
			.GetByIdAsync(topic.ServiceBusId, cancellationToken);

		return GetSubscriptions(topic.AzureSubscription, serviceBusNamespace, topic.Name, cancellationToken);
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
				TopicSubscriptions = GetSubscriptions(subscription, serviceBusNamespace, topic.Name, cancellationToken)
			};
		}
	}

	private static async IAsyncEnumerable<TopicSubscription> GetSubscriptions(
		IAzureSubscription subscription, IServiceBusNamespace serviceBusNamespace, string topicName,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var topic = await serviceBusNamespace.Topics
			.GetByNameAsync(topicName, cancellationToken);
		var subscriptions = await topic.Subscriptions
			.ListAsync(true, cancellationToken);

		foreach (var topicSubscription in subscriptions.OrderBy(s => s.Name))
		{
			yield return new TopicSubscription(subscription, serviceBusNamespace, topicName, topicSubscription);
		}
	}
}