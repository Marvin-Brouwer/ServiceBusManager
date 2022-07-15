using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Components;

namespace MarvinBrouwer.ServiceBusManager.Services;

public sealed class AzureLandscapeRenderingService
{
	private readonly TreeView _azureLandscape;
	private readonly IAzureSubscriptionService _subscriptionService;
	private readonly IAzureServiceBusService _serviceBusService;

	public AzureLandscapeRenderingService(
		TreeView azureLandscape,
		IAzureSubscriptionService subscriptionService,
		IAzureServiceBusService serviceBusService)
	{
		_azureLandscape = azureLandscape;
		_subscriptionService = subscriptionService;
		_serviceBusService = serviceBusService;
	}

	public async IAsyncEnumerable<SubscriptionTreeViewItem> LoadSubscriptions(
		Action? doneCallBack, [EnumeratorCancellation] CancellationToken cancellationToken)
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

			subscriptionTreeViewItem.Loaded += async (_,_) =>
			{
				var isLast = subscriptionTreeViewItem.Subscription == subscriptions.Last() ;
					
				await LoadSubscriptionContents(subscriptionTreeViewItem, isLast ? doneCallBack : null, cancellationToken);
			};

			yield return subscriptionTreeViewItem;
			_azureLandscape.Items.Add(subscriptionTreeViewItem);
		}
	}

	public async Task LoadSubscriptionContents(
		SubscriptionTreeViewItem subscriptionTreeViewItem, Action? doneCallBack, CancellationToken cancellationToken)
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

			var isLast = serviceBusTreeViewItem.ServiceBus == serviceBuses.Last();
			await LoadServiceBusResources(serviceBusTreeViewItem, isLast ? doneCallBack : null, cancellationToken);
		}

		subscriptionTreeViewItem.IsEnabled = true;
	}

	public async Task LoadServiceBusResources(ServiceBusTreeViewItem serviceBusTreeViewItem,
		Action? doneCallBack, CancellationToken cancellationToken)
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
			doneCallBack?.Invoke();
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

			var isLast = topicTreeViewItem.Topic == topicList.Last();
			await LoadTopicSubscriptions(topicTreeViewItem, isLast ?  doneCallBack : null, cancellationToken);
		}

		serviceBusTreeViewItem.IsEnabled = true;
	}

	public async Task LoadTopicSubscriptions(
		TopicTreeViewItem topicTreeViewItem, Action? doneCallBack, CancellationToken cancellationToken)
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
		doneCallBack?.Invoke();
	}
}
