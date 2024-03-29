using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Components;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.Fluent;

namespace MarvinBrouwer.ServiceBusManager.Services;

/// <inheritdoc />
public sealed class AzureLandscapeRenderingService : IAzureLandscapeRenderingService
{
	private readonly TreeView _azureLandscape;
	private readonly IAzureSubscriptionService _subscriptionService;
	private readonly IAzureServiceBusService _serviceBusService;

	/// <inheritdoc cref="AzureLandscapeRenderingService" />
	public AzureLandscapeRenderingService(
		TreeView azureLandscape,
		IAzureSubscriptionService subscriptionService,
		IAzureServiceBusService serviceBusService)
	{
		_azureLandscape = azureLandscape;
		_subscriptionService = subscriptionService;
		_serviceBusService = serviceBusService;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<AzureSubscriptionTreeViewItem> LoadSubscriptions(
		List<AzureConnection> azureConnections, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var hasCleared = false;
		var subscriptionTasks = new List<Task>();

		foreach (var azureConnection in azureConnections)
		{
			var subscriptions = await _subscriptionService
				.ListSubscriptions(azureConnection, cancellationToken)
				.ToListAsync(cancellationToken);

			foreach (var subscription in subscriptions)
			{
				if (!hasCleared) _azureLandscape.Items.Clear();
				hasCleared = true;

				var subscriptionTreeViewItem = new AzureSubscriptionTreeViewItem(subscription);

				yield return subscriptionTreeViewItem;
				_azureLandscape.Items.Add(subscriptionTreeViewItem);
				var azureClient = azureConnection.AzureClient.WithSubscription(subscription.SubscriptionId);
				subscriptionTasks.Add(LoadSubscriptionContents(azureClient, subscriptionTreeViewItem, cancellationToken));
			}
		}

		await Task.WhenAll(subscriptionTasks);
	}

	/// <inheritdoc />
	public async Task LoadSubscriptionContents(IAzure azure,
		AzureSubscriptionTreeViewItem azureSubscriptionTreeViewItem, CancellationToken cancellationToken)
	{
		azureSubscriptionTreeViewItem.IsEnabled = false;
		azureSubscriptionTreeViewItem.IsExpanded = false;
		var serviceBuses = await _serviceBusService
			.ListServiceBuses(azure, azureSubscriptionTreeViewItem.Subscription, cancellationToken)
			.ToListAsync(cancellationToken);
		
		var hasCleared = false;
		var serviceBusTasks = new List<Task>();
		foreach (var serviceBus in serviceBuses)
		{
			if (!hasCleared)
			{
				azureSubscriptionTreeViewItem.Items.Clear();
				azureSubscriptionTreeViewItem.IsEnabled = true;
				hasCleared = true;
			}

			var serviceBusTreeViewItem = new ServiceBusTreeViewItem(serviceBus);
			azureSubscriptionTreeViewItem.Items.Add(serviceBusTreeViewItem);

			serviceBusTasks.Add(LoadServiceBusResources(azure, serviceBusTreeViewItem, cancellationToken));
		}

		azureSubscriptionTreeViewItem.IsEnabled = true;
		await Task.WhenAll(serviceBusTasks);
	}

	/// <inheritdoc />
	public async Task LoadServiceBusResources(IAzure azure,
		ServiceBusTreeViewItem serviceBusTreeViewItem, CancellationToken cancellationToken)
	{
		serviceBusTreeViewItem.IsEnabled = false;
		serviceBusTreeViewItem.IsExpanded = false;
		var (queues, topics) = await _serviceBusService
			.ListServiceBusResources(azure, serviceBusTreeViewItem.ServiceBus, cancellationToken);

		var queueList = await queues
			.ToListAsync(cancellationToken);
		var topicList = await topics
			.ToListAsync(cancellationToken);

		var hasCleared = false;

		foreach (var queue in queueList)
		{
			if (!hasCleared)
			{
				serviceBusTreeViewItem.Items.Clear();
				serviceBusTreeViewItem.IsEnabled = true;
				hasCleared = true;
			}

			var queueTreeViewItem = new QueueTreeViewItem(queue);
			serviceBusTreeViewItem.Items.Add(queueTreeViewItem);
		}

		if (!topicList.Any())
		{
			serviceBusTreeViewItem.IsEnabled = true;
			return;
		}

		var topicTasks = new List<Task>();
		foreach (var topic in topicList)
		{
			if (!hasCleared)
			{
				serviceBusTreeViewItem.Items.Clear();
				serviceBusTreeViewItem.IsEnabled = true;
				hasCleared = true;
			}

			var topicTreeViewItem = new TopicTreeViewItem(topic);
			serviceBusTreeViewItem.Items.Add(topicTreeViewItem);

			topicTasks.Add(LoadTopicSubscriptions(azure, topicTreeViewItem, cancellationToken));
		}

		serviceBusTreeViewItem.IsEnabled = true;
		await Task.WhenAll(topicTasks);
	}

	/// <inheritdoc />
	public async Task LoadTopicSubscriptions(IAzure azure,
		TopicTreeViewItem topicTreeViewItem, CancellationToken cancellationToken)
	{
		topicTreeViewItem.IsEnabled = false;
		topicTreeViewItem.IsExpanded = false;
		var topicSubscriptions = await _serviceBusService
			.ListTopicSubscriptions(azure, topicTreeViewItem.Topic, cancellationToken);

		var hasCleared = false;

		await foreach (var topicSubscription in topicSubscriptions.WithCancellation(cancellationToken))
		{
			if (!hasCleared)
			{
				topicTreeViewItem.Items.Clear();
				topicTreeViewItem.IsEnabled = true;
				hasCleared = true;
			}

			var topicSubscriptionTreeViewItem = new TopicSubscriptionTreeViewItem(topicSubscription, topicTreeViewItem.Topic);
			topicTreeViewItem.Items.Add(topicSubscriptionTreeViewItem);
		}

		topicTreeViewItem.IsEnabled = true;
	}
}
