using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.ServiceBus.Management;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public sealed class AzureServiceBusService : IAzureServiceBusService
{
	private readonly IAzureServiceBusClientFactory _clientFactory;

	public AzureServiceBusService(IAzureServiceBusClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
	}


	public IEnumerable<Task<ServiceBus>> ListServiceBuses(List<string> secrets, CancellationToken cancellationToken)
	{
		foreach (string secret in secrets)
		{
			yield return GetServiceBus(secret, cancellationToken);
		}
	}

	public async Task<ServiceBus> GetServiceBus(string secret, CancellationToken cancellationToken)
	{
		var managementClient = await _clientFactory.GetManagementClient(secret, cancellationToken);
		var namespaceInfo = await managementClient.GetNamespaceInfoAsync(cancellationToken);

		var serviceBus = new ServiceBus(
			namespaceInfo.Name, secret
		);

		serviceBus.Topics = await GetTopics(managementClient, serviceBus, cancellationToken);
		serviceBus.Queues = await GetQueues(managementClient, serviceBus, cancellationToken);

		return serviceBus;
	}

	private static async Task<IEnumerable<Topic>> GetTopics(ManagementClient managementClient, ServiceBus serviceBus, CancellationToken cancellationToken)
	{
		var topics = await managementClient.GetTopicsAsync(AzureConstants.ServiceBusResourceMaxItemCount, 0, cancellationToken);
		return await Task.WhenAll(topics
			.Select(async topicDescription =>
			{
				var topic = new Topic(serviceBus, topicDescription.Path);
				var subscriptions = await managementClient.GetSubscriptionsAsync(topicDescription.Path, AzureConstants.ServiceBusResourceMaxItemCount, 0, cancellationToken);
				topic.Subscriptions = subscriptions.Select(subscription => new TopicSubscription(topic, subscription.SubscriptionName));
				return topic;
			}));

	}

	private static async Task<IEnumerable<Queue>> GetQueues(ManagementClient managementClient, ServiceBus serviceBus, CancellationToken cancellationToken)
	{
		var queues = await managementClient.GetQueuesAsync(AzureConstants.ServiceBusResourceMaxItemCount, 0, cancellationToken);
		return queues.Select(queue => new Queue(serviceBus, queue.Path));
	}
}