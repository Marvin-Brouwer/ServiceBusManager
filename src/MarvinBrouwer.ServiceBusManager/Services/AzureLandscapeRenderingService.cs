using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Components;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

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
	public async IAsyncEnumerable<SubscriptionTreeViewItem> LoadSubscriptions(
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var subscriptions = await _subscriptionService
			.ListSubscriptions(cancellationToken)
			.ToListAsync(cancellationToken);
		
		var hasCleared = false;
		foreach (var subscription in subscriptions)
		{
			if (!hasCleared) _azureLandscape.Items.Clear();
			hasCleared = true;

			var subscriptionTreeViewItem = new SubscriptionTreeViewItem(subscription);
			
			yield return subscriptionTreeViewItem;
			_azureLandscape.Items.Add(subscriptionTreeViewItem);
		}

		foreach (SubscriptionTreeViewItem subscriptionTreeViewItem in _azureLandscape.Items)
		{
			await LoadSubscriptionContents(subscriptionTreeViewItem, cancellationToken);
		}
	}

	/// <inheritdoc />
	public async Task LoadSubscriptionContents(
		SubscriptionTreeViewItem subscriptionTreeViewItem, CancellationToken cancellationToken)
	{
		subscriptionTreeViewItem.IsEnabled = false;
		subscriptionTreeViewItem.IsExpanded = false;
		var serviceBuses = await _serviceBusService
			.ListServiceBuses(subscriptionTreeViewItem.Subscription, cancellationToken)
			.ToListAsync(cancellationToken);
		
		var hasCleared = false;
		foreach (var serviceBus in serviceBuses)
		{
			if (!hasCleared)
			{
				subscriptionTreeViewItem.Items.Clear();
				subscriptionTreeViewItem.IsEnabled = true;
				hasCleared = true;
			}

			var serviceBusTreeViewItem = new ServiceBusTreeViewItem(serviceBus);
			subscriptionTreeViewItem.Items.Add(serviceBusTreeViewItem);
			
			await LoadServiceBusResources(serviceBusTreeViewItem, cancellationToken);
		}
		
		subscriptionTreeViewItem.IsEnabled = true;
	}

	/// <inheritdoc />
	public async Task LoadServiceBusResources(
		ServiceBusTreeViewItem serviceBusTreeViewItem, CancellationToken cancellationToken)
	{
		serviceBusTreeViewItem.IsEnabled = false;
		serviceBusTreeViewItem.IsExpanded = false;
		var (queues, topics) = _serviceBusService
			.ListServiceBusResources(serviceBusTreeViewItem.ServiceBus, cancellationToken);

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
			
			await LoadTopicSubscriptions(topicTreeViewItem, cancellationToken);
		}
		
		serviceBusTreeViewItem.IsEnabled = true;
	}

	/// <inheritdoc />
	public async Task LoadTopicSubscriptions(
		TopicTreeViewItem topicTreeViewItem, CancellationToken cancellationToken)
	{
		topicTreeViewItem.IsEnabled = false;
		topicTreeViewItem.IsExpanded = false;
		var topicSubscriptions = _serviceBusService.ListTopicSubscriptions(topicTreeViewItem.Topic, cancellationToken);

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
