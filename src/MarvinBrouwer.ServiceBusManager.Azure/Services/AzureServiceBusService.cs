using System.Runtime.CompilerServices;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.Services.AppAuthentication;
using ISubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;

namespace MarvinBrouwer.ServiceBusManager.Azure.Services;

public sealed class AzureServiceBusService : IAzureServiceBusService
{
	private readonly IAzureAuthenticationService _authenticationService;

	public AzureServiceBusService(IAzureAuthenticationService authenticationService)
	{
		_authenticationService = authenticationService;
	}

	public async IAsyncEnumerable<ServiceBus> ListServiceBuses(ISubscription subscription, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var credentials = await _authenticationService.Authenticate(cancellationToken);
		var azure = credentials.WithSubscription(subscription.SubscriptionId);

		var resourceGroups = await azure.ResourceGroups
			.ListAsync(true, cancellationToken);
		
		foreach (var resourceGroup in resourceGroups)
		{
			var serviceBusNameSpaces = await azure.ServiceBusNamespaces
				.ListByResourceGroupAsync(resourceGroup.Name, true, cancellationToken);

			foreach (var serviceBusNamespace in serviceBusNameSpaces)
			{
				yield return new ServiceBus(serviceBusNamespace);
			}
		}

	}

	public (IAsyncEnumerable<Queue> queues, IAsyncEnumerable<Topic> topics) ListServiceBusResources(ServiceBus serviceBus, CancellationToken cancellationToken)
	{
		var queues = GetQueues(serviceBus.InnerResource, cancellationToken);
		var topics = GetTopics(serviceBus.InnerResource, cancellationToken);

		return (queues, topics);
	}

	public IAsyncEnumerable<TopicSubscription> ListTopicSubscriptions(Topic topic, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		return GetSubscriptions(topic.InnerResource, cancellationToken);
	}

	private static async IAsyncEnumerable<Queue> GetQueues(IServiceBusNamespace serviceBusNamespace, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var queues = await serviceBusNamespace.Queues
			.ListAsync(true, cancellationToken);

		foreach (IQueue queue in queues)
		{
			yield return new Queue(queue);
		}
	}

	private static async IAsyncEnumerable<Topic> GetTopics(IServiceBusNamespace serviceBusNamespace, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var topics = await serviceBusNamespace.Topics
			.ListAsync(true, cancellationToken);

		foreach (var topic in topics)
		{
			yield return new Topic(topic)
			{
				TopicSubscriptions = GetSubscriptions(topic, cancellationToken)
			};
		}
	}

	private static async IAsyncEnumerable<TopicSubscription> GetSubscriptions(ITopic topic, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var subscriptions = await topic.Subscriptions
			.ListAsync(true, cancellationToken);

		foreach (var topicSubscription in subscriptions)
		{
			yield return new TopicSubscription(topicSubscription);
		}
	}
}