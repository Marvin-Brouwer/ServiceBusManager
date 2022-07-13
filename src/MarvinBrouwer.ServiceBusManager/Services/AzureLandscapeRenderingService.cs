using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

	public async IAsyncEnumerable<SubscriptionTreeViewItem> LoadSubscriptions([EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var subscriptions = _subscriptionService.ListSubscriptions(cancellationToken);

		var hasCleared = false;
		await foreach (var subscription in subscriptions.WithCancellation(cancellationToken))
		{
			if (!hasCleared) _azureLandscape.Items.Clear();
			hasCleared = true;

			var subscriptionTreeViewItem = new SubscriptionTreeViewItem(subscription);

			subscriptionTreeViewItem.Loaded += async (_,_) => await LoadSubscriptionContents(subscriptionTreeViewItem, cancellationToken);

			yield return subscriptionTreeViewItem;
			_azureLandscape.Items.Add(subscriptionTreeViewItem);
		}
	}

	public async Task LoadSubscriptionContents(SubscriptionTreeViewItem subscriptionTreeViewItem, CancellationToken cancellationToken)
	{
		subscriptionTreeViewItem.IsEnabled = false;
		var serviceBuses = _serviceBusService.ListServiceBuses(subscriptionTreeViewItem.Subscription, cancellationToken);
		
		var hasCleared = false;
		await foreach (var serviceBus in serviceBuses.WithCancellation(cancellationToken))
		{
			if (!hasCleared) subscriptionTreeViewItem.Items.Clear();
			hasCleared = true;

			var serviceBusTreeViewItem = new ServiceBusTreeViewItem(serviceBus);
			subscriptionTreeViewItem.Items.Add(serviceBusTreeViewItem);

			await LoadServiceBusResources(serviceBusTreeViewItem, cancellationToken);
		}
		subscriptionTreeViewItem.IsEnabled = true;
	}

	public async Task LoadServiceBusResources(ServiceBusTreeViewItem serviceBusTreeViewItem,
		CancellationToken cancellationToken)
	{
		serviceBusTreeViewItem.IsEnabled = false;
		var (queues, topics) = _serviceBusService.ListServiceBusResources(serviceBusTreeViewItem.ServiceBus, cancellationToken);

		serviceBusTreeViewItem.Items.Clear();

		await foreach (var queue in queues.WithCancellation(cancellationToken))
		{
			var queueTreeViewItem = new QueueTreeViewItem(queue);
			serviceBusTreeViewItem.Items.Add(queueTreeViewItem);
		}

		await foreach (var topic in topics.WithCancellation(cancellationToken))
		{
			var topicTreeViewItem = new TopicTreeViewItem(topic);
			serviceBusTreeViewItem.Items.Add(topicTreeViewItem);

			await LoadTopicSubscriptions(topicTreeViewItem, cancellationToken);
		}

		serviceBusTreeViewItem.IsEnabled = true;
	}

	public async Task LoadTopicSubscriptions(TopicTreeViewItem topicTreeViewItem, CancellationToken cancellationToken)
	{
		topicTreeViewItem.IsEnabled = false;
		var topicSubscriptions = _serviceBusService.ListTopicSubscriptions(topicTreeViewItem.Topic, cancellationToken);

		topicTreeViewItem.Items.Clear();

		await foreach (var topicSubscription in topicSubscriptions.WithCancellation(cancellationToken))
		{
			var topicSubscriptionTreeViewItem = new TopicSubscriptionTreeViewItem(topicSubscription, topicTreeViewItem.Topic);
			topicTreeViewItem.Items.Add(topicSubscriptionTreeViewItem);
		}

		topicTreeViewItem.IsEnabled = true;
	}
}
