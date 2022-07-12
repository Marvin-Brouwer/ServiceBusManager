using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using MarvinBrouwer.ServiceBusManager.Azure.Services;
using MarvinBrouwer.ServiceBusManager.Components;
using MarvinBrouwer.ServiceBusManager.Services;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace MarvinBrouwer.ServiceBusManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly AzureSubscriptionService _subscriptionService;
	private readonly AzureAuthenticationService _azureAuthenticationService;
	private readonly CancellationTokenSource _applicationCancellationSource;
	private readonly AzureServiceBusService _serviceBusService;
	private readonly AzureLandscapeRenderingService _azureLandscapeRenderingService;
	private readonly LocalStorageService _localStorageService;

	private CancellationToken CancellationToken => _applicationCancellationSource.Token;

	public MainWindow()
	{
		InitializeComponent();
		ClearStatusPanel();
		AppendStatusMessage(@"Loading data...");
		SelectedItemChanged(null);

		_applicationCancellationSource = new CancellationTokenSource();
		_azureAuthenticationService = new AzureAuthenticationService();
		_subscriptionService = new AzureSubscriptionService(_azureAuthenticationService);
		_serviceBusService = new AzureServiceBusService(_azureAuthenticationService);
		_azureLandscapeRenderingService = new AzureLandscapeRenderingService(AzureLandscape, _subscriptionService, _serviceBusService);
		_localStorageService = new LocalStorageService();

		Closing += (_, _) => _applicationCancellationSource.Cancel();
		Cursor = Cursors.Wait;
		MenuItemReload.IsEnabled = false;
		
		Loaded += WindowLoaded;
	}

	#region WindowUtilities
	private void AppendStatusMessage(string message)
	{
		if (string.IsNullOrWhiteSpace(message))
			StatusBox.Children.Add(new HeaderedContentControl { Header = Environment.NewLine });
		StatusBox.Children.Add(new HeaderedContentControl { Header = message });
	}

	private void ClearStatusPanel()
	{
		StatusBox.Children.Clear();
	}
	#endregion

	private async void WindowLoaded(object sender, RoutedEventArgs e)
	{
		await _azureAuthenticationService.Authenticate(CancellationToken);
		_localStorageService.PrepareDownloadFolder();
		Cursor = Cursors.Arrow;
		await LoadFullAzureLandscape();
	}

	private async void ReloadAzureLandscape(object sender, RoutedEventArgs e)
	{
		ClearStatusPanel();
		AppendStatusMessage(@"Loading data...");
		await LoadFullAzureLandscape();
	}

	private async Task LoadFullAzureLandscape()
	{
		AzureLandscape.IsEnabled = false;
		SelectedItemChanged(null);
		foreach (SubscriptionTreeViewItem treeViewItem in AzureLandscape.Items)
		{
			treeViewItem.IsEnabled = false;
			treeViewItem.IsExpanded = false;
		}
		AzureLandscape.IsEnabled = true;
		CheckAzureLandscapeState();

		var subscriptionTreeViewItems = _azureLandscapeRenderingService.LoadSubscriptions(CancellationToken);
		await foreach (var subscriptionTreeViewItem in subscriptionTreeViewItems.WithCancellation(CancellationToken))
		{
			subscriptionTreeViewItem.IsEnabledChanged += (_, _) => CheckAzureLandscapeState();
		}

		CheckAzureLandscapeState();
	}

	private bool AzureLandscapeIsLoading =>
		AzureLandscape.Items.Count == 0 ||
		!AzureLandscape.Items.Cast<SubscriptionTreeViewItem>().All(treeViewItem => treeViewItem.IsEnabled);

	private void CheckAzureLandscapeState()
	{
		AzureLandscape.Cursor = AzureLandscapeIsLoading ? Cursors.AppStarting : Cursors.Arrow;
		MenuItemReload.IsEnabled = !AzureLandscapeIsLoading;

		if (!AzureLandscapeIsLoading) AppendStatusMessage(@"Done!");
	}

	private async void ReloadItem(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is null) return;
		if (AzureLandscape.SelectedItem is not BaseTreeViewItem { CanReload: true } baseTreeViewItem) return;

		baseTreeViewItem.IsEnabled = false;
		baseTreeViewItem.IsExpanded = false;

		if (AzureLandscape.SelectedItem is SubscriptionTreeViewItem subscriptionTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadSubscriptionContents(subscriptionTreeViewItem, CancellationToken);
		}

		if (AzureLandscape.SelectedItem is ServiceBusTreeViewItem serviceBusTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadServiceBusResources(serviceBusTreeViewItem, CancellationToken);
		}

		if (AzureLandscape.SelectedItem is TopicTreeViewItem topicTreeViewItem)
		{
			await _azureLandscapeRenderingService.LoadTopicSubscriptions(topicTreeViewItem, CancellationToken);
		}
	}

	private void OpenDownloadFolder(object sender, RoutedEventArgs e)
	{
		_localStorageService.OpenDownloadFolder();
	}

	private void OpenGitHubRepository(object sender, RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = "https://github.com/Marvin-Brouwer/ServiceBusManager",
			Verb = "open",
			UseShellExecute = true
		});
	}

	private void ShowReadmeWindow(object sender, RoutedEventArgs e)
	{
		// TODO Readme window
		throw new System.NotImplementedException();
	}

	private void ShowRequeueDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not BaseTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanRequeue) return;

		// TODO
		throw new System.NotImplementedException();
	}

	private void ShowDownloadDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not BaseTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanDownload) return;

		// TODO
		throw new System.NotImplementedException();
	}

	private void ShowUploadDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not BaseTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanUpload) return;

		// TODO
		throw new System.NotImplementedException();
	}

	private void ShowClearDialog(object sender, RoutedEventArgs e)
	{
		if (AzureLandscape.SelectedItem is not BaseTreeViewItem treeViewItem) return;
		if (!treeViewItem.CanClear) return;

		// TODO
		throw new System.NotImplementedException();
	}

	private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (AzureLandscape.SelectedItem is null)
			SelectedItemChanged(null);
		else if (AzureLandscape.SelectedItem is BaseTreeViewItem treeViewItem)
			SelectedItemChanged(treeViewItem);
		else SelectedItemChanged(null);
	}

	private void SelectedItemChanged(BaseTreeViewItem? treeViewItem)
	{
		ButtonClear.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanClear;
		ButtonDownload.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanDownload;
		ButtonRequeue.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanRequeue;
		ButtonUpload.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanUpload;

		MenuItemReloadSelected.IsEnabled = treeViewItem is not null && !AzureLandscapeIsLoading
			&& treeViewItem.IsEnabled && treeViewItem.CanReload;

		CommandManager.InvalidateRequerySuggested();
	}
}